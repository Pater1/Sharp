using SharpStreamServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using Newtonsoft.Json;

namespace Sharp {
    public static class PartyTracker {
        #region All this should eventually be put on disk
        private static Dictionary<string, PartyServer> directory = new Dictionary<string, PartyServer>();
        #endregion

        public static PartyServer GetByKey(string partyKey) {
            if (directory.ContainsKey(partyKey)) {
                return directory[partyKey];
            }
            return null;
        }

        public static List<String> Keys = new List<string>();
        private static readonly RNGCryptoServiceProvider rnjesus = new RNGCryptoServiceProvider();
        private const int bytesPerLong = 8; //bytes in a long
        public static void StartParty(string username) {
            //byte[] keyRaw = new byte[bytesPerLong];
            //long key = 0;
            //do {
            //    rnjesus.GetBytes(keyRaw);
            //    key = ConcantonateBytes(keyRaw);
            //} while (keys.Contains(key)); //Makes absolutely sure no keys are reused (though that should never happen anyway)


            if(directory.ContainsKey(username))
            {
                PartyTracker.GetByKey
                directory.Remove(username);
            }

            PartyServer p = new PartyServer(username);
            if(Keys.Contains(username))
            {
                Keys.Remove(username);
            }
            Keys.Add(username);

            directory.Add(username, p);
        }

        public static void Despose(PartyServer of) {
            //keys.Remove(of._Key);
            directory.Remove(of._Key);
        }

        private static long ConcantonateBytes(byte[] bytes) {
            Array.Resize(ref bytes, bytesPerLong);
            long ret = 0;
            for (int i = 0; i < bytes.Length; i++) {
                unchecked {
                    ret |= ((long)bytes[i]) << (8 * i);
                }
            }
            return ret;
        }
    }
}