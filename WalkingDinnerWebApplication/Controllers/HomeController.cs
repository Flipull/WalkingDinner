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
            for (int i = 0; i < 250; i++)
                context.CreateRandomDuo();
            

            for (int i = 0; i < 100; i++)
            {
                var rand_choice = dice.Next(10, 99).ToString();
                var duos = context.Duos.Where(d => d.PostCode.StartsWith(rand_choice))//split duos in regions to show 'more real' scenarios
                                        .Take(dice.Next(8,128) )
                                        .ToList();

                //var duos = context.SelectRandomDuos(dice.Next(8, Math.Min(64, context.Duos.Count()) ));
                context.CreateRandomPlan(duos);
            }

            for (int i = 0; i < 50; i++)
            {
                var nieuwschema = context.BruteForceSchemaSalesmanProblem(context.EventPlannen.First());
            }
            */

            context.Dispose();
            return View();
        }

        public ActionResult ImportDb()
        {
            var context = new WalkingDinnerContext();

            System.Diagnostics.Debug.WriteLine($"emptying PostcodeGeoLocationCaches");
            context.Database.ExecuteSqlCommand("delete from PostcodeGeoLocationCaches");
            for (int i = 1; i <= 8; i++)
            {
                System.Diagnostics.Debug.WriteLine($"importing file #{i}");

                var file_content = System.IO.File.ReadLines(Server.MapPath($"~\\App_Data\\dbo.PostcodeGeoLocationCaches.data_{i}.sql"));
                var trans = context.Database.BeginTransaction();
                foreach (var line in file_content)
                {
                    context.Database.ExecuteSqlCommand(line);
                }
                trans.Commit();
            }

            return new HttpNotFoundResult("Import gelukt; Geen view om weer te geven");
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