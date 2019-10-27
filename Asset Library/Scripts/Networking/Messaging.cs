using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BaD.Modules.Networking {
    /// <summary>
    /// This class handles sending messages and receiving messages sent by Photon.
    /// </summary>
    public abstract class Messaging: MonoBehaviourPunCallbacks, IOnEventCallback {

        //
        //public enum NetMessageCodes { TextMessage = 15, InventoryMessage = 10 };

        /// <summary>
        /// This byte value will be the first step in filtering out non related messages.
        /// </summary>
        [Tooltip("The specific code used to filter messages of this type.")]
        public byte MessageCode;

        /// <summary>
        /// How many received message ids to cache. This system is in place to prevent multiple execution of the same messages.
        /// If you are expecting a large amount of traffic, it is suggestable to use a larger cache length.
        /// </summary>
        [Tooltip("How long the buffer for tracking sent messages should be.")]
        public int RequestNumberCacheLength;

        /// <summary>
        /// If a PhotonView component is also present on the same game object, the messaging class can optionally include a filter
        /// view ids.
        /// </summary>
        [Tooltip("If on, will also filter messages by an attatched Photon View Component.")]
        public bool FilterMessagesByView;

        /// <summary>
        /// An option for setting who can receive the message. Seems to be limited to sending to everyone,
        /// master client only, or others.
        /// </summary>
        public ReceiverGroup Receivers;

        /// <summary>
        /// How the photon server will deal with caching this message.
        /// </summary>
        public EventCaching CachingOption;

        /// <summary>
        /// If set, photon will send multiple copies of this message to ensure it is received.
        /// The messaging class is already set up to ignore subsiquent copies of the same message.
        /// </summary>
        public bool ReliabilityMode;

        /// <summary>
        /// Keeps track of how many messages have been sent.
        /// </summary>
        private int RequestNumber = 0;

        /// <summary>
        /// An array of all messages that have been received.
        /// </summary>
        private int[] AcceptedRequests;

        /// <summary>
        /// The index of the last message id received.
        /// </summary>
        private int lastRequestIndex = 0;

        /// <summary>
        /// Makes sure that the message meta data type is only registered once.
        /// </summary>
        public static bool RegisteredMM = false;

        /// <summary>
        /// If a PhotonView component is present, this returns the View ID of that component.
        /// </summary>
        public int ViewID {
            get {
                if (FilterMessagesByView && GetComponent<PhotonView>() != null) {
                    return GetComponent<PhotonView>().ViewID;
                }
                return 0;
            }
        }

        public virtual void Awake () {
            AcceptedRequests = new int[RequestNumberCacheLength];
            for (int i = 0; i < AcceptedRequests.Length; i++) {
                AcceptedRequests[i] = -1;
            }
            if (!RegisteredMM) {
                PhotonPeer.RegisterType(typeof(MessageMeta), (byte) 'Z', MessageMeta.Serialize, MessageMeta.DeSerialize);
                RegisteredMM = false;
            }
        }

        public void Start () {
            int baseMessageNumber = PhotonNetwork.LocalPlayer.ActorNumber * 100000;
            RequestNumber = baseMessageNumber + ViewID*100;
        }

        /// <summary>
        /// This is executed when a message is received by photon. This should not be overridden unless that is your intent.
        /// Override the abstract MessageReceived function if you need to act on received messages.
        /// </summary>
        /// <param name="photonEvent">The message data from the event.</param>
        public void OnEvent ( EventData photonEvent ) {
            if (photonEvent.Code != MessageCode) return;//Not an inventory command, skip the remainder of this.
            Debug.Log("Relavent message gotten  in " + gameObject.name + " .");
            object[] data = (object[]) photonEvent.CustomData;
            MessageMeta mm = (MessageMeta) data[0];
            if (!AcceptRequest(mm.MessageID)) { return; }//Only returns if the message was already processed
            if (FilterMessagesByView && mm.ViewID != ViewID) { return; }//returns if the messaging has been set to filter by view and the view does not match
            MessageReceived(data);
        }

        /// <summary>
        /// Sends a message using photon and the set options from the class.
        /// </summary>
        /// <param name="data">An array of all message data. Must follow the photon object serialization guidelines.</param>
        /// <returns>The ID of the network message.</returns>
        public int SendNetMessage ( object[] data ) {
            object[] newData;
            newData = new object[data.Length + 1];
            newData[0] = new MessageMeta(RequestNumber, ViewID);
            for (int i = 1; i < newData.Length; i++) {
                newData[i] = data[i - 1];
            }
            RequestNumber++;
            //A reuqest to synch the inventory with others has been received.
            RaiseEventOptions eventOptions = new RaiseEventOptions {
                Receivers = Receivers,
                CachingOption = CachingOption
            };
            SendOptions sendOptions = new SendOptions {
                Reliability = ReliabilityMode
            };
            
            if (PhotonNetwork.OfflineMode) {
                //In offline mode
                StartCoroutine(OfflineModeResponse(newData));
            } else {
                //Not in offline mode
                Debug.Log("I'm sending a message!");
                PhotonNetwork.RaiseEvent(MessageCode, (object)newData, eventOptions, sendOptions);
            }
            return ((MessageMeta) newData[0]).MessageID;//Give back a request code!!
        }

        public abstract void MessageReceived ( object[] messageData );

        private bool AcceptRequest ( int requestNumber ) {
            foreach (int req in AcceptedRequests) {
                if (requestNumber == req) {
                    return false;//This request was already accepted.
                }
            }
            //This request has not been accepted yet.
            AcceptedRequests[lastRequestIndex] = requestNumber;
            lastRequestIndex++;
            if (lastRequestIndex >= AcceptedRequests.Length) {
                lastRequestIndex = 0;
            }
            return true;
        }


        IEnumerator OfflineModeResponse(object[] message) {
            yield return new WaitForSeconds(0.015f);//Wait a reasonable amount of time to simulate network latency
            MessageReceived(message);
        }
    }

    public class MessageMeta {
        public int MessageID;
        public int ViewID;

        public MessageMeta ( int MessageID, int ViewID ) {
            this.MessageID = MessageID;
            this.ViewID = ViewID;
        }

        public static byte[] Serialize ( object received ) {
            MessageMeta mm = (MessageMeta) received;
            byte[] intBytes1 = BitConverter.GetBytes(mm.MessageID);
            byte[] intBytes2 = BitConverter.GetBytes(mm.ViewID);
            //if (BitConverter.IsLittleEndian) { Array.Reverse(intBytes1); Array.Reverse(intBytes2); }

            byte[] combined = new byte[intBytes1.Length + intBytes2.Length];
            Array.Copy(intBytes1, combined, intBytes1.Length);
            Array.Copy(intBytes2, 0, combined, intBytes1.Length, intBytes2.Length);
            return combined;
        }

        public static object DeSerialize ( byte[] received ) {
            MessageMeta mm = new MessageMeta(0, 0) {
                MessageID = BitConverter.ToInt32(received, 0),
                ViewID = BitConverter.ToInt32(received, 4)
            };
            return mm;
        }
    }
}