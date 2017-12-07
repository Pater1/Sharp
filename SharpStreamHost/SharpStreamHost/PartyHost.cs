using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.FileFormats;
using NAudio.Codecs;
using NAudio.Utils;
using NAudio.Wave.Compression;
using System.Threading;
using System.IO;
using SharpStreamServer;
using SharpStreamExtentions;

namespace SharpStreamHost {
    public class PartyHost: IDisposable {
        //private volatile SampleToWaveProvider byteConverter;
        private volatile ISampleProvider source;
        public ISampleProvider Source {
            get {
                return source;
            }
            set {
                source = value;
            }
        }
        
        private volatile float[] buffer;
        private PartyServer server;

        private bool service = true;
        
        private void PushStream(object o = null) {
            if (buffer != null) {
                int size = source.Read(buffer, 0, buffer.Length);
                float[] sizeAdjustedBuffer = buffer.Resize(size);
                //string data = sizeAdjustedBuffer.EncodeSamples();
                //server.Push(data);

                bool full = server.Push(sizeAdjustedBuffer);

                double d = (double)sizeAdjustedBuffer.Length / (source.WaveFormat.AverageBytesPerSecond/4);
                double e = d * 1000;
                double f = e - 0;
                int delay = (int)f;
                delay = delay < 0 ? 0 : delay;
                if (full) {
                    Thread.Sleep(delay);
                }
            }

            if (service) {
                ThreadPool.QueueUserWorkItem(PushStream);
            }
        }

        public void Dispose() {
            service = false;
        }

        public PartyHost(){
            ThreadPool.QueueUserWorkItem(PushStream);
        }
        public PartyHost(string file, PartyServer server) : this(new AudioFileReader(file), server) {
            //buffer = new float[server.chunkSize];
            buffer = new float[1000];
        }
        public PartyHost(ISampleProvider _source, PartyServer server): this() {
            Source = _source;
            this.server = server;
            server.WaveFormat = source.WaveFormat;
        }


    }
}
