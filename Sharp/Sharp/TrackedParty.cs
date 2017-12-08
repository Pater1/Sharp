using SharpStreamServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using Newtonsoft.Json;
using SharpStreamHost;
using System.IO;
using Sharp.Controllers;

namespace Sharp {
    public static class PartyTracker {
        #region All this should eventually be put on disk
        private static Dictionary<string, PartyServer> directory = new Dictionary<string, PartyServer>();
        public static List<string> Keys = new List<string>();
        #endregion

        public static PartyServer GetByKey(string partyKey) {
            if (directory.ContainsKey(partyKey)) {
                return directory[partyKey];
            }
            return null;
        }
        
        public static void StartParty(string username) {
            DesposeOf(username);

            QueueableParty ph = new QueueableParty();
            PartyServer p = new PartyServer(username);
            p.OnDispose += ph.Dispose;
            p.TrackedHost = ph;
            ph.Server = p;

            //string[] strs = Directory.GetFiles(HttpContext.Current.Server.MapPath(@"~/TestFiles/"));
            //ph.AddSource(strs);

            if (Keys.Contains(username))
            {
                Keys.Remove(username);
            }
            Keys.Add(username);

            directory.Add(username, p);
        }

        public static void DesposeOf(string username) {
            if (directory.ContainsKey(username)) {
                directory[username].Dispose();
                directory.Remove(username);
            }
        }
        
    }
}