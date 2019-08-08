using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaD.Chopknee.Utilities {

    public class Choptilities {
        public static void DestroyList(List<GameObject> gos) {
            foreach (GameObject go in gos) {
                GameObject.Destroy(go);
            }
        }

        public static short ByteToShort(byte byte1, byte byte2) {
            return (short) ( ( byte2 << 8 ) | ( byte1 << 0 ) );
        }

        public static void ShortToBytes(short number, out byte byte1, out byte byte2) {
            byte2 = (byte) ( number >> 8 );
            byte1 = (byte) ( number >> 0 );
        }
    }
}