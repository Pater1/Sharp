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
        public static List<string> Keys = new List<string>();
        #endregion

        public static PartyServer GetByKey(string partyKey) {
            if (directory.ContainsKey(partyKey)) {
                return directory[partyKey];
            }
            return null;
        }
        
        private const int bytesPerLong = 8; //bytes in a long
        public static void StartParty(string username) {
            DesposeOf(username);

            PartyServer p = new PartyServer(username);
            Keys.Add(username);

            directory.Add(username, p);
        }

        public static void DesposeOf(string username) {
            if (directory.ContainsKey(username)) {
                directory[username].Dispose();
                directory.Remove(username);
            }
        }

        //private static long ConcantonateBytes(byte[] bytes) {
        //    Array.Resize(ref bytes, bytesPerLong);
        //    long ret = 0;
        //    for (int i = 0; i < bytes.Length; i++) {
        //        unchecked {
        //            ret |= ((long)bytes[i]) << (8 * i);
        //        }
        //    }
        //    return ret;
        //}
    }
}