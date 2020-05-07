using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WalkingDinnerWebApplication.DAL;
using WalkingDinnerWebApplication.Models;
using WalkingDinnerWebApplication.ViewModels;

namespace WalkingDinnerWebApplication.Controllers
{
    public class EventPlanController : Controller
    {
        private WalkingDinnerContext db = new WalkingDinnerContext();

        // GET: EventPlan
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id == null)
                return HttpNotFound();
            
            EventPlan plan = db.EventPlannen.Find((int)id);
            if (plan == null)
                return HttpNotFound();


            var planvm = new EventPlanViewModel()
            {
                Id = plan.Id,
                AantalDeelnemers = plan.AantalDeelnemers,
                AantalDuosPerGroep = plan.AantalDuosPerGroep,
                AantalGangen = plan.AantalGangen,
                AantalGroepen = plan.AantalGroepen,
                Naam = plan.Naam,
                IngeschrevenDuos = new List<DuoDetailsViewModel>()
            };
            foreach(var duo in plan.IngeschrevenDuos)
            {
                planvm.IngeschrevenDuos.Add(new DuoDetailsViewModel()
                {
                    Adres = $"{duo.Straat} {duo.Huisnummer}",
                    Naam = duo.Naam,
                    Postcode = duo.PostCode,
                    Stad = duo.Stad
                });
            }
            
            return View(planvm);
        }


        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(int? aantal_duos, int? aantal_duospergroep, int? aantal_gangen, string naam)
        {
            DatabaseSeed engine = new DatabaseSeed(db);
            EventPlan plan = engine.CreatePlan((int)aantal_duos, (int)aantal_duospergroep, (int)aantal_gangen, naam);
            if (plan == null)
                return HttpNotFound();
            else
                return RedirectToAction("Details", new { id = plan.Id } );
        }
    }
}