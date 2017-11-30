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

        public string PullChunk(long? partyKey, int? samplesRequested, int? channelCount) {
            if (!partyKey.HasValue || !samplesRequested.HasValue || !channelCount.HasValue) {
                return new BoomBox();
            }
            return PartyTracker.GetByKey(partyKey.Value).Pull(samplesRequested.Value, channelCount.Value);
        }

        public string PullFormat(long? partyKey) {
            if (!partyKey.HasValue) {
                return "{}";
            }
            return JsonConvert.SerializeObject(PartyTracker.GetByKey(partyKey.Value).WaveFormat);
        }
    }
}