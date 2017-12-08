using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using NAudio.Wave;
using SharpStreamHost;
using System.IO;

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

        public string PullChunk(string partyKey, int? samplesRequested, int? channelCount, int? readHead) {
            if (string.IsNullOrEmpty(partyKey) || !samplesRequested.HasValue || !channelCount.HasValue) {
                return new BoomBox();
            }
            return PartyTracker.GetByKey(partyKey).Pull(samplesRequested.Value, channelCount.Value, readHead);
        }

        public string PullFormat(string partyKey) {
            if (string.IsNullOrEmpty(partyKey)) {
                return "{}";
            }
            return JsonConvert.SerializeObject(PartyTracker.GetByKey(partyKey).WaveFormat);
        }

        [HttpPost]
        public ActionResult AddSource(string partyKey, HttpPostedFileBase file)  
        {
            string _path = null;
            try {
                if (file.ContentLength > 0) {
                    string _FileName = Path.GetFileName(file.FileName);
                    _path = Path.Combine(Server.MapPath("~/UploadCache"), _FileName);
                    file.SaveAs(_path);
                }
                ViewBag.Message = "File Uploaded Successfully!!";
            } catch {
                ViewBag.Message = "File upload failed!!";
            }
            if(_path != null)PartyTracker.GetByKey(partyKey)?.TrackedHost?.AddSource(_path);
            return RedirectToAction("Party", "Home", new { id = partyKey });
        }
    }
}