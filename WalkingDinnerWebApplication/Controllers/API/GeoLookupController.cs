using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WalkingDinnerWebApplication.DAL;

namespace WalkingDinnerWebApplication.Controllers.API
{
    public class GeoLookupController : Controller
    {
        private WalkingDinnerContext db = new WalkingDinnerContext();
        // GET: GeoLookup
        public ActionResult Index(string postcode, int? huisnr)
        {
            if (postcode == null || huisnr == null)
                return new JsonResult();

            DatabaseSeed seed = new DatabaseSeed(db);
            
            var res = new JsonResult();
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            res.Data = seed.PostcodeToGeoLocation(postcode, (int)huisnr);
            return res;
        }
    }
}