using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WalkingDinnerWebApplication.DAL;
using WalkingDinnerWebApplication.ModelMappingExtensions;
using WalkingDinnerWebApplication.Models;
using WalkingDinnerWebApplication.ViewModels;

namespace WalkingDinnerWebApplication.Controllers
{
    public class EventSchemaController : Controller
    {
        WalkingDinnerContext db = new WalkingDinnerContext();

        public ActionResult Index()
        {
            EventSchemaIndexViewModel viewModel = new EventSchemaIndexViewModel
            {
                EventSchemas = db.EventSchemas.ToList()
            };

            return View(viewModel);
        }

        // GET: EventSchema/Details/{id}
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            EventSchema schema = db.EventSchemas.Find(id);

            if(schema == null)
            {
                return HttpNotFound();
            }

            EventSchemaViewModel viewModel = schema.ToViewModel(db);
            
            return View(viewModel);
        }
    }
}