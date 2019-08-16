using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BaD.Modules.Networking {
    public abstract class Messaging: MonoBehaviourPunCallbacks, IOnEventCallback {

        public enum NetMessageCodes { TextMessage = 15, InventoryMessage = 10 };

        [Tooltip("The specific code used to filter messages of this type.")]
        public byte MessageCode;
        [Tooltip("How long the buffer for tracking sent messages should be.")]
        public int RequestNumberCacheLength;
        [Tooltip("If on, will also filter messages by an attatched Photon View Component.")]
        public bool FilterMessagesByView;

        public ReceiverGroup Receivers;
        public EventCaching CachingOption;
        public bool ReliabilityMode;
        [SerializeField]
        private int RequestNumber = 0;
        private int[] AcceptedRequests;
        private int lastRequestIndex = 0;

        //A special data type that is used for the messaging class
        public static bool RegisteredMM = false;

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

        public void OnEvent ( EventData photonEvent ) {
            if (photonEvent.Code != MessageCode) return;//Not an inventory command, skip the remainder of this.

            object[] data = (object[]) photonEvent.CustomData;
            MessageMeta mm = (MessageMeta) data[0];
            if (!AcceptRequest(mm.MessageID)) { return; }//Only returns if the message was already processed
            if (FilterMessagesByView && mm.ViewID != ViewID) { return; }//returns if the messaging has been set to filter by view and the view does not match
            MessageReceived(data);
        }

        //inserts the request number to the data, then sends the message to all others listening.
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
            PhotonNetwork.RaiseEvent(MessageCode, (object) newData, eventOptions, sendOptions);
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