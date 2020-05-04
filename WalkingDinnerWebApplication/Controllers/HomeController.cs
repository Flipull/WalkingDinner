using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WalkingDinnerWebApplication.DAL;
using WalkingDinnerWebApplication.Migrations;

namespace WalkingDinnerWebApplication.Controllers
{
    public class HomeController : Controller
    {
        WalkingDinnerContext db = new WalkingDinnerContext();

        public ActionResult Index()
        {
            /*
            //TESTCODE
            var seedData = new DatabaseSeed(db);
            var dice = new Random();

            for (int i = 0; i < 250; i++)
                seedData.CreateRandomDuo();
            

            for (int i = 0; i < 100; i++)
            {
                var rand_choice = dice.Next(10, 99).ToString();
                var duos = db.Duos.Where(d => d.PostCode.StartsWith(rand_choice)) // split duos in regions to show 'more real' scenarios
                                        .Take(dice.Next(8,128) )
                                        .ToList();

                //var duos = db.SelectRandomDuos(dice.Next(8, Math.Min(64, db.Duos.Count()) ));
                seedData.CreateRandomPlan(duos);
            }

            for (int i = 0; i < 50; i++)
            {
                var nieuwschema = seedData.BruteForceSchemaSalesmanProblem(db.EventPlannen.First());
            }
            */

            return View();
        }

        public ActionResult ImportDb()
        {
            System.Diagnostics.Debug.WriteLine($"emptying PostcodeGeoLocationCaches");
            db.Database.ExecuteSqlCommand("delete from PostcodeGeoLocationCaches");
            for (int i = 1; i <= 8; i++)
            {
                System.Diagnostics.Debug.WriteLine($"importing file #{i}");

                var file_content = System.IO.File.ReadLines(Server.MapPath($"~\\App_Data\\dbo.PostcodeGeoLocationCaches.data_{i}.sql"));
                var trans = db.Database.BeginTransaction();
                foreach (var line in file_content)
                {
                    db.Database.ExecuteSqlCommand(line);
                }
                trans.Commit();
            }

            return new HttpNotFoundResult("Import gelukt; Geen view om weer te geven");
        }
    }
}