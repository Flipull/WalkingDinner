using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WalkingDinnerLibrary;
using WalkingDinnerWebApplication.Migrations;

namespace WalkingDinnerWebApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var context = new WalkingDinnerContext();

            var tst = new List<int>();
            for (int i = 1; i < 100; i++)
            {
                var stramienen_count = EventStramien.CreateMogelijkStramien(i).Count;
                tst.Add(stramienen_count);
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}