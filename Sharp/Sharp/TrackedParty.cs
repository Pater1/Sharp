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
        public static List<long> keys { get; private set; } = new List<long>();
        private static Dictionary<long, PartyServer> directory = new Dictionary<long, PartyServer>();
        #endregion

        public static PartyServer GetByKey(long partyKey) {
            if (directory.ContainsKey(partyKey)) {
                return directory[partyKey];
            }
            return null;
        }

        private static readonly RNGCryptoServiceProvider rnjesus = new RNGCryptoServiceProvider();
        private const int bytesPerLong = 8; //bytes in a long
        public static long StartParty() {
            byte[] keyRaw = new byte[bytesPerLong];
            long key = 0;
            do {
                rnjesus.GetBytes(keyRaw);
                key = ConcantonateBytes(keyRaw);
            } while (keys.Contains(key)); //Makes absolutely sure no keys are reused (though that should never happen anyway)

            PartyServer p = new PartyServer(key);

            keys.Add(key);
            directory.Add(key, p);

            return key;
        }

        public static void Despose(PartyServer of) {
            keys.Remove(of._Key);
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