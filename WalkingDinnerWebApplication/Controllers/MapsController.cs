using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WalkingDinnerWebApplication.DAL;
using WalkingDinnerWebApplication.ViewModels;

namespace WalkingDinnerWebApplication.Controllers
{
    public class MapsController : Controller
    {
        WalkingDinnerContext db = new WalkingDinnerContext();

        // GET: Maps/Index/{id}
        public ActionResult Index(uint? id)
        {
            if (id == null)
                return HttpNotFound();

            var schema = db.EventSchemas
                                    .Where(s => s.Id == id)
                                    .FirstOrDefault();
            if (schema == null)
                return HttpNotFound();

            var seed = new DatabaseSeed(db);
            var pathing = seed.CalculateSchemaPathing(schema);
            
            MapViewModel vm = new MapViewModel()
            {
                DuoData = pathing,
                AantalDeelnemers = schema.AantalDeelnemers,
                AantalGangen = schema.AantalGangen,
                AantalGroepen = schema.AantalGroepen,
                AantalDuosPerGroep = schema.AantalDuosPerGroep,
                Naam = schema.Naam,
            };

            // TODO: Read https://blog.jongallant.com/2012/10/do-i-have-to-call-dispose-on-dbcontext/
            // db.Dispose();

            return View(vm);
        }
    }
}