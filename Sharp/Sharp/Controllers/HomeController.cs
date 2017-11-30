using SharpStreamHost;
using SharpStreamServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sharp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public static PartyServer ps;
        public ActionResult Party(long? id) {//id = partyKey
            id = PartyTracker.StartParty();
            ps = PartyTracker.GetByKey(id.Value);

            string[] strs = Directory.GetFiles(@"C:\Users\Donaven Vandeveer\Desktop\Sharp\Sharp\SharpStreamHost\SharpStreamHost_Console\TestFiles");
            string sng = strs[(new Random()).Next(0, strs.Length)];

            PartyHost host = new PartyHost(sng, ps);

            if ( !id.HasValue                             //didn't ask for a party
              || !PartyTracker.keys.Contains(id.Value)      //asked for a party that doesn't exist
            ) {
                //return 'browse parties' view
                return RedirectToAction("Index");
            } else {
                return View(ps);
            }
        }

        public ActionResult NewParty() {//id = partyKey
            long id = PartyTracker.StartParty();
            
            //return 'browse parties' view
            return RedirectToAction("Index", id);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}