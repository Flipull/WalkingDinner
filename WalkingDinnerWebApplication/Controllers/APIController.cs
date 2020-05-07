using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WalkingDinnerWebApplication.DAL;

namespace WalkingDinnerWebApplication.Controllers
{
    public class APIController : Controller
    {
        private WalkingDinnerContext db = new WalkingDinnerContext();
        // GET: GeoLookup
        public ActionResult GeoLookup(string postcode, int? huisnr)
        {
            if (postcode == null || huisnr == null)
                return new JsonResult();

            DatabaseSeed seed = new DatabaseSeed(db);
            
            var res = new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = seed.PostcodeToGeoLocation(postcode, (int)huisnr)
            };
            
            return res;
        }

        public ActionResult GetStramienen(int? duo_count)
        {
            if (duo_count == null)
                return new JsonResult();

            DatabaseSeed engine = new DatabaseSeed(db);
            
            var stramienen = EventStramien.CreateMogelijkStramien((int)duo_count);

            var res = new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = stramienen
            };
            return res;
        }
    }
}