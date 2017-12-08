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
        private volatile ISampleProvider source;
        public ISampleProvider Source {
            get {
                return source;
            }
            set {
                source = value;
                if(server != null) {
                    server.WaveFormat = Source.WaveFormat;
                }
            }
        }


        private volatile float[] buffer;
        private PartyServer server;
        public PartyServer Server {
            get {
                return server;
            }
            set {
                server = value;
                if (Source != null) {
                    server.WaveFormat = Source.WaveFormat;
                }
            }
        }
        public virtual void AddSource(string path) { }
        public virtual void AddSource(ISampleProvider source) { }

        private bool service = true;

        protected delegate void ServiceStream();
        protected event ServiceStream OnPush, OnEmpty;
        private void PushStream(object o = null) {
            if (buffer != null) {
                if(OnPush != null) OnPush();

                if (source == null) {
                    if (OnEmpty != null) OnEmpty();
                } else {
                    int size = source.Read(buffer, 0, buffer.Length);
                    if (buffer.Length > 0 && size <= 0) {
                        if (OnEmpty != null) OnEmpty();
                    } else {
                        float[] sizeAdjustedBuffer = buffer.Resize(size);

                        bool full = Server.Push(sizeAdjustedBuffer);

                        double d = (double)sizeAdjustedBuffer.Length / (source.WaveFormat.AverageBytesPerSecond / 4);
                        double e = d * 1000;
                        double f = e - 0;
                        int delay = (int)f;
                        delay = delay < 0 ? 0 : delay;
                        if (full) {
                            Thread.Sleep(delay);
                        }
                    }
                }
            } else {
                buffer = new float[1000];
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
            buffer = new float[1000];
        }
        public PartyHost(ISampleProvider _source, PartyServer server): this() {
            Source = _source;
            this.Server = server;
            server.WaveFormat = source.WaveFormat;
        }
        
    }
}
