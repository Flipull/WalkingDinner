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

            var schema = context.EventSchemas
                                    .Where(s => s.Id == id)
                                    .FirstOrDefault();
            if (schema == null)
                return HttpNotFound();
            
            var pathing = context.CalculateSchemaPathing(schema);
            
            MapViewModel vm = new MapViewModel()
            {
                DuoData = pathing,
                AantalDeelnemers = schema.AantalDeelnemers,
                AantalGangen = schema.AantalGangen,
                AantalGroepen = schema.AantalGroepen,
                AantalDuosPerGroep = schema.AantalDuosPerGroep,
                Naam = schema.Naam,
            };

            context.Dispose();
            return View(vm);
        }
    }
}