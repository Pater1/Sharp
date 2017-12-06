using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using NAudio.Wave;
using SharpStreamHost;

namespace Sharp.Controllers {
    public class APIController : Controller {
        [HttpPost]
        public string PushChunk(long? partyKey, string pushedChunk) { //Validate that only party-owning dj can access; return false on fail
            if (!partyKey.HasValue || pushedChunk == null) {
                return new BoomBox();
            }
            return new BoomBox() {
                Succeeded = true
            };
        }

        public string PullChunk(string partyKey, int? samplesRequested, int? channelCount) {
            if (string.IsNullOrEmpty(partyKey) || !samplesRequested.HasValue || !channelCount.HasValue) {
                return new BoomBox();
            }
            return PartyTracker.GetByKey(partyKey).Pull(samplesRequested.Value, channelCount.Value);
        }

        public string PullFormat(string partyKey) {
            if (string.IsNullOrEmpty(partyKey)) {
                return "{}";
            }
            return JsonConvert.SerializeObject(PartyTracker.GetByKey(partyKey).WaveFormat);
        }
    }
}