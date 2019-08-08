using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace BaD.Modules.Utility {
    public static class Serializer {

        public static byte[] SerializeObject ( object obj ) {
            if (obj != null) {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
            return null;
        }

        public static object DeserializeObject ( byte[] bytes ) {
            using (MemoryStream ms = new MemoryStream()) {
                BinaryFormatter binForm = new BinaryFormatter();
                ms.Write(bytes, 0, bytes.Length);
                ms.Seek(0, SeekOrigin.Begin);
                object obj = (object) binForm.Deserialize(ms);
                return obj;
            }
        }

    }
}
