using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpStreamHost;
using NAudio;
using NAudio.Codecs;
using NAudio.CoreAudioApi;
using NAudio.Dmo;
using NAudio.Dsp;
using NAudio.FileFormats;
using NAudio.Gui;
using NAudio.Midi;
using NAudio.Mixer;
using NAudio.SoundFont;
using NAudio.Utils;
using NAudio.Wave;
using NAudio.Wave.Asio;
using NAudio.Wave.SampleProviders;
using SharpStreamClient;
using SharpStreamServer;
using System.IO;

namespace SharpStreamHost_Console {
    class Program {
        static void Main(string[] args) {
            PartyServer server = new PartyServer("");

            string[] strs = Directory.GetFiles(@"..\..\TestFiles\");
            string sng = strs[(new Random()).Next(0, strs.Length)];
            Console.WriteLine(sng);

            PartyHost host = new PartyHost(sng, server);

            PartyGuest guest = new PartyGuest(server);

            Console.Write("Press [Enter] to exit...");
            Console.Read();
        }
    }
}
