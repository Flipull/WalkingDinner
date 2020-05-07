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
    public class DuoController : Controller
    {
        WalkingDinnerContext db = new WalkingDinnerContext();

        // GET: Duo/Details/{id}
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Duo duo = db.Duos.Find(id);

            if (duo == null)
            {
                return HttpNotFound();
            }

            DuoDetailsViewModel viewModel = duo.ToDetailsViewModel(db);
            
            return View(viewModel);
        }
    }
}