using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaD.Chopknee.Utilities {
    public class Choptilities {
        public static void DestroyList(List<GameObject> gos) {
            foreach (GameObject go in gos) {
                GameObject.Destroy(go);
            }
            gos.Clear();
        }

        public static short ByteToShort(byte byte1, byte byte2) {
            return (short) ( ( byte2 << 8 ) | ( byte1 << 0 ) );
        }

        public static void ShortToBytes(short number, out byte byte1, out byte byte2) {
            byte2 = (byte) ( number >> 8 );
            byte1 = (byte) ( number >> 0 );
        }

        public static Vector2 ClampVector2 ( Vector2 a, Vector2 minimum, Vector2 maximum ) {
            return new Vector2(Mathf.Clamp(a.x, minimum.x, maximum.x), Mathf.Clamp(a.y, minimum.y, maximum.y));
        }

        public static Vector2 WorldToScreenPointProjected ( Camera camera, Vector3 worldPos ) {
            Vector3 camNormal = camera.transform.forward;
            Vector3 vectorFromCam = worldPos - camera.transform.position;
            float camNormDot = Vector3.Dot(camNormal, vectorFromCam);
            if (camNormDot <= 0) {
                // we are behind the camera forward facing plane, project the position in front of the plane
                Vector3 proj = ( camNormal * camNormDot * 1.01f );
                worldPos = camera.transform.position + ( vectorFromCam - proj );
            }
            return RectTransformUtility.WorldToScreenPoint(camera, worldPos);
        }
    }
}