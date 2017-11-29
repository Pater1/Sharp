using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SharpStreamHost {
    public static class ByteEncoder {
        public static string EncodeBytes(this byte[] data) {
            return Convert.ToBase64String(data);
        }
        public static byte[] DecodeBytes(this string data) {
            if (data == null) return null;
            return Convert.FromBase64String(data);
        }

        public static string EncodeSamples(this float[] data) {
            return JsonConvert.SerializeObject(data);
        }
        public static float[] DecodeSamples(this string data) {
            if (data == null) return null;
            return JsonConvert.DeserializeObject<float[]>(data);
        }
    }
}
