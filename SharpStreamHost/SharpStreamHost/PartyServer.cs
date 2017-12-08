using NAudio.Utils;
using NAudio.Wave;
using SharpStreamExtentions;
using SharpStreamHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpStreamServer {
    public class PartyServer: IDisposable {
        private volatile WaveFormat waveFormat;
        public WaveFormat WaveFormat {
            get {
                return waveFormat;
            }
            set {
                waveFormat = value;
                //bufferedWaveProvider = new BufferedWaveProvider(waveFormat);
            }
        }
        
        private void ServiceStream(object o = null) {}
        
        public String _Key { get; }
        private CircularSampleBuffer buffer;

        public PartyServer(string key) {
            _Key = key;
            buffer = new CircularSampleBuffer(44100 * 60 * 5);//5min of CD-quality audio
            ThreadPool.QueueUserWorkItem(ServiceStream);
        }

        public bool Push(float[] data) {
            buffer.Write(data, 0, data.Length);
            return buffer.IsFull(44100 * 10);
        }
        public int Pull(float[] data, int offset = 0) {
            return buffer.Read(data, offset, data.Length);
        }
        public int Pull(float[/*samples*/][/*channels*/] data, int offset = 0) {
            float[] readIn = new float[data[0].Length * data.Length];
            int cnt = buffer.Read(readIn, offset, readIn.Length);
            for(int i = 0; i < readIn.Length; i++) {
                int c = i % data.Length;
                int l = i / data.Length;

                if(readIn[i] != 0) {
                    bool burner = true;
                }
                float tmp = readIn[i];
                data[c][l] = tmp;
            }
            return cnt;
        }
        public BoomBox Pull(int sampleCount, int channelCount = 1, bool getFormat = true) {
            BoomBox b = new BoomBox(sampleCount) {
                RequestWaveFormat = getFormat
            };

            float[] ac = b.AudioChunk;
            Array.Resize(ref ac, sampleCount);
            b.AudioChunk = ac;

            return Pull(b);
        }
        public BoomBox Pull(BoomBox b) {
            //buffer.readPosition = b.ReadPosition;

            int read = Pull(b.AudioChunk, 0);
            b.ReadPosition = buffer.readPosition;

            if (b.RequestWaveFormat) {
                b.Format = WaveFormat;
            }

            b.Succeeded = true;

            return b;
        }
        
        public event Action<PartyServer> OnDispose;
        public void Dispose() {
            OnDispose(this);
        }
    }
}