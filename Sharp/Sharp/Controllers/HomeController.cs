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


        public ActionResult CreateParty()
        { 
            PartyTracker.StartParty(User.Identity.Name); 

            return View("Party", PartyTracker.GetByKey(User.Identity.Name));
        }

        public ActionResult Party(string id)
        {
            if(PartyTracker.GetByKey(id) == null)
            {
                return View("Index");
            }

            return View("Party", PartyTracker.GetByKey(id));
        }

        public ActionResult StopParty()
        {
            PartyTracker.DesposeOf(User.Identity.Name);
            return View("Index");
        }
    }
}