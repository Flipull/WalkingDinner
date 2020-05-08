using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WalkingDinnerWebApplication.DAL;
using WalkingDinnerWebApplication.ModelMappingExtensions;
using WalkingDinnerWebApplication.Models;
using WalkingDinnerWebApplication.ViewModels;

namespace WalkingDinnerWebApplication.Controllers
{
    public class EventPlanController : Controller
    {
        private WalkingDinnerContext db = new WalkingDinnerContext();

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id == null)
                return HttpNotFound();
            
            EventPlan plan = db.EventPlannen.Find((int)id);
            if (plan == null)
                return HttpNotFound();

            var planvm = plan.ToViewModel();
            
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

        [HttpPost]
        public ActionResult Cancel(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var plan = db.EventPlannen.Find((int)id);
            if (plan == null)
                return HttpNotFound();

            db.EventPlannen.Remove(plan);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult CreateSchema(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var plan = db.EventPlannen.Find((int)id);
            if (plan == null)
                return HttpNotFound();

            if (plan.AantalDeelnemers != plan.IngeschrevenDuos.Count)
                return RedirectToAction("Details", new { id = id });

            var engine = new DatabaseSeed(db);

            var schema = engine.BruteForceSchemaSalesmanProblem(plan);

            if (schema == null)
                return HttpNotFound();
            
            return RedirectToAction("Details","EventSchema", new { id = schema.Id });
        }

    }
}