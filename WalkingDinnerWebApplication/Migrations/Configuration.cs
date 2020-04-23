namespace WalkingDinnerWebApplication.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WalkingDinnerWebApplication.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<WalkingDinnerWebApplication.WalkingDinnerContext>
    {
        static Random dice = new Random();
        static readonly string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N",
                                            "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "WalkingDinnerWebApplication.WalkingDinnerContext";
        }

        protected override void Seed(WalkingDinnerWebApplication.WalkingDinnerContext context)
        {
            context.Duos.RemoveRange(context.Duos);
            context.EventPlannen.RemoveRange(context.EventPlannen);
            context.SaveChanges();

            if (context.Duos.Count() > 0)
                return;
            for (int i = 0; i < 1000; i++)
            {
                context.Duos.Add(CreateRandomDuo());
            }
            context.SaveChanges();
            var duolist = context.Duos.ToList();
            
            //maak 50 random plannen met [6..50] random duos
            for (int i = 0; i < 50; i++)
            {
                var selected_duos = new List<Duo>();
                var duo_count = dice.Next(6, 50);
                for (int j = 0; j < duo_count; j++)
                {
                    //var duo_to_add = duolist[0];
                    var duo_to_add = duolist[dice.Next(duolist.Count)];
                    while (selected_duos.Contains(duo_to_add))
                    {
                        //duo_to_add = duolist[0];
                        duo_to_add = duolist[dice.Next(duolist.Count)];
                    }
                    selected_duos.Add(duo_to_add);
                }

                var new_plan = CreateRandomPlan(selected_duos);
                //als het plannen niet gefaald is door verkeerd aantal duos, voeg plan toe
                if (new_plan != null)
                    context.EventPlannen.Add(new_plan);
            };
            context.SaveChanges();
        }

        protected EventPlan CreateRandomPlan(ICollection<Duo> duos)
        {
            var stramienen = EventStramien.CreateMogelijkStramien(duos.Count);
            if (stramienen.Count == 0)
                return null;
            
            var gekozen_stramien = stramienen[stramienen.Count];

            var plan = new EventPlan()
            {
                Naam = "Grappige Naam voor een Eentje",
                AantalDeelnemers = duos.Count,
                AantalGangen = dice.Next(EventStramien.MIN_GANGEN, gekozen_stramien.MaxGangen),
                AantalGroepen = gekozen_stramien.Groepen,
                AantalDuosPerGroep = gekozen_stramien.Groepgrootte,
                IngeschrevenDuos = new List<Duo>()
            };

            foreach(var duo in duos)
            {
                plan.IngeschrevenDuos.Add(duo);
                //duo.IngeschrevenPlannen.Add(plan);
            }
            return plan;
        }
        protected Duo CreateRandomDuo()
        {

            Duo newduo = new Duo();
            newduo.PostCode = dice.Next(1000, 9999) + alpha[dice.Next(alpha.Length - 1)] + alpha[dice.Next(alpha.Length - 1)];
            //newduo.GeoLocation...
            newduo.Naam = "Pseudoniem";
            newduo.Huisnummer = (dice.Next(0, 10) * 2) + "";
            
            return newduo;
        }

        protected Tuple<double, double> PostcodeToGeoLocation(string postcode)
        {
            return null;
        }

    }



    public class EventStramien 
    {
        public static readonly byte MIN_GANGEN = 2;
        public static readonly byte MAX_GANGEN = 4;
        public int MaxGangen { get; set; }
        public int Groepen { get; set; }
        public int Groepgrootte { get; set; }

        static public List<EventStramien> CreateMogelijkStramien(int deelnemers_count)
        {
            var stramienen = new List<EventStramien>();
            for (int i = 1; i <= deelnemers_count/2; i++)
            {
                if (deelnemers_count % i != 0) continue;
                
                var groep_count = i;
                var groep_grootte = deelnemers_count / i;

                
                if (groep_grootte < 2 || groep_grootte > 4)
                    continue;
                if (groep_count < 2 * groep_grootte - 1)
                    continue;
                

                stramienen.Add(new EventStramien() { 
                        MaxGangen = Math.Min(groep_grootte, Math.Min(groep_count, MAX_GANGEN)), 
                        Groepen = groep_count, 
                        Groepgrootte = groep_grootte 
                });
            }
            return stramienen;
        }
    }
}
