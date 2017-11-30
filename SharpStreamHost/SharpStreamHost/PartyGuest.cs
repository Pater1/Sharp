using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Threading;
using SharpStreamServer;
using SharpStreamExtentions;
using SharpStreamHost;

namespace SharpStreamClient
{
    public class PartyGuest {
        //public const int defaultBuffer = 44100 * 10; //10 seconds of CD quality audio
        
        private volatile IWavePlayer player;
        public IWavePlayer Player {
            get {
                return player;
            }
            set {
                player = value;
            }
        }

        private volatile float[] bufferRaw = new float[100];
        private volatile SampleWaveProvider byteConverter;
        private volatile BufferedSampleProvider source;
        public BufferedSampleProvider Source {
            get {
                return source;
            }
            set {
                source = value;
                if (value != null) {
                    byteConverter = new SampleWaveProvider(source);

                    bufferWave = new BufferedWaveProvider(source.WaveFormat);
                } else {
                    byteConverter = null;

                    bufferWave = null;
                }
            }
        }

        private volatile BufferedWaveProvider bufferWave;

        private volatile bool listenable = false;
        private ulong currentChunk = 0;

        private void Listen(object o = null) {
            while (true) {
                if (listenable) {
                    int l = server.Pull(bufferRaw, 0);
                    while (Source.IsFull(bufferRaw.Length)) Thread.Sleep(100);
                    if (l > 0) {
                        Source.AddSamples(bufferRaw, 0, l);
                        player.Play();
                    }
                }
            }
        }
        PartyServer server;
        private PartyGuest() {}

        public PartyGuest(PartyServer server): this() {
            this.server = server;
            listenable = true;

            Source = new BufferedSampleProvider(server.WaveFormat);

            player = new WaveOut();
            player.Init(Source);

            ThreadPool.QueueUserWorkItem(Listen);
        }
    }
}
