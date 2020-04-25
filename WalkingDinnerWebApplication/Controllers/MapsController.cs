using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WalkingDinnerWebApplication.ViewModels;

namespace WalkingDinnerWebApplication.Controllers
{
    public class MapsController : Controller
    {
        // GET: Maps
        public ActionResult Index(uint? id)
        {
            if (id ==null)
                return HttpNotFound();

            var context = new WalkingDinnerContext();

            var any_schema = context.EventSchemas
                                    .Where(s => s.Id == id)
                                    .FirstOrDefault();
            if (any_schema == null)
                return HttpNotFound();
            
            var pathing = context.CalculateSchemaPathing(any_schema);
            
            MapViewModel vm = new MapViewModel()
            {
                DuoData = pathing
            };

            return View(vm);
        }
    }
}