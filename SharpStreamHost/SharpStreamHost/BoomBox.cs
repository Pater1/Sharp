using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpStreamHost {
    public class BoomBox {
        public bool Succeeded { get; set; }
        public float[] AudioChunk { get; set; }
        public int ReadPosition { get; set; }

        public bool RequestWaveFormat { get; set; }
        public WaveFormat Format { get; set; } //usually null to save bandwidth

        public static implicit operator string(BoomBox ths) {
            string ret =  JsonConvert.SerializeObject(ths);
            return ret;
        }

        public BoomBox(int audioChunkSize = 0, int channels = 0) {
            Succeeded = false;
            //var tmp = new List<float[]>();
            //for(int i = 0; i < channels; i++) {
            //    tmp.Add(new float[audioChunkSize]);
            //}
            //AudioChunk = tmp.ToArray();
            AudioChunk = new float[audioChunkSize];
            ReadPosition = 0;

            RequestWaveFormat = false;
            Format = null;
        }
    }
}
