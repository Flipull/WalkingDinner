using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WalkingDinnerWebApplication.Migrations;

namespace WalkingDinnerWebApplication.Controllers
{
    public class HomeController : Controller
    {
        static private Random dice = new Random();
        public ActionResult Index()
        {
            var context = new WalkingDinnerContext();
            /*
            //TESTCODE
            for (int i = 0; i < 200; i++)
                context.CreateRandomDuo();

            for (int i = 0; i < 20; i++)
            {
                var duos = context.SelectRandomDuos(dice.Next(8, context.Duos.Count() ));
                context.CreateRandomPlan(duos);
            }
            */

            var nieuwschema = context.BruteForceSchemaSalesmanProblem(context.EventPlannen.First());
            
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