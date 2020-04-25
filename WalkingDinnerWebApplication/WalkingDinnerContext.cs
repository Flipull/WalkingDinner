﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WalkingDinnerWebApplication.Models;

namespace WalkingDinnerWebApplication
{
    public class WalkingDinnerContext : DbContext
    {
        public WalkingDinnerContext() :
            base("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=WalkingDinner;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Duo>()
                .HasMany<Groep>(d => d.GeplandeEventGroepen)
                .WithMany(g => g.Gasten);
            modelBuilder.Entity<Groep>()
                .HasRequired<Gang>(g => g.Gang)
                .WithMany(g => g.Groepen)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<Gang>()
                .HasRequired<EventSchema>(g => g.Schema)
                .WithMany(s => s.Gangen)
                .WillCascadeOnDelete(true);

            base.OnModelCreating(modelBuilder);
        }
        
        public DbSet<Duo> Duos { get; set; }
        public DbSet<EventPlan> EventPlannen { get; set; }
        public DbSet<EventSchema> EventSchemas { get; set; }
        public DbSet<Gang> Gangen { get; set; }
        public DbSet<Groep> Groepen { get; set; }



        public DbSet<PostcodeGeoLocationCache> PostcodeGeoLocationCaches { get; set; }


        private Random dice = new Random();
        public PostcodeGeoLocationCache RandomGeoLocation()
        {
            //TODO: stop hanging when you forgot to fill the database-table
            PostcodeGeoLocationCache loc = null;
            do
            {
                var x = dice.Next(110, 999).ToString();
                loc = PostcodeGeoLocationCaches.Where(c => c.Postcode.Contains(x))
                .FirstOrDefault();
            } while (loc == null);
            return loc;
        }
        public PostcodeGeoLocationCache PostcodeToGeoLocation(string postcode, int home_nr)
        {
            //if cached data exists, return it
            var caches = PostcodeGeoLocationCaches
                    .Where(o => o.Postcode == postcode.ToUpper() && 
                                o.NummerMin <= home_nr && o.NummerMax >= home_nr &&
                                (
                                    (o.NummerType == "mixed") ||
                                    (o.NummerType == "even" && home_nr%2==0) ||
                                    (o.NummerType == "odd" && home_nr%2 == 1)
                                )
                                
                            ).ToList();

            if (caches.Count == 0)
                return null;
            if (caches.Count > 1)
                throw new ArgumentOutOfRangeException("");


            var cache_obj = caches[0];
            if (cache_obj != null)
                return cache_obj;
            else
                return null;//no remote API check (for now?)
        }


        static readonly string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N",
                                            "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        public Duo CreateRandomDuo()
        {

            Duo newduo = new Duo();
            PostcodeGeoLocationCache geoloc = RandomGeoLocation();

            newduo.PostCode = geoloc.Postcode;
            newduo.GeoLong = geoloc.GeoLong;
            newduo.GeoLat = geoloc.GeoLat;
            newduo.Adres = geoloc.Straat;
            var ar = geoloc.Straat.Split();
            var naam = ar[ar.Length - 1]
                    .Replace("straat", "")
                    .Replace("plein", "")
                    .Replace("hof", "")
                    .Replace("laan", "")
                    .Replace("erf", "")
                    .Replace("weg", "")
                    .Replace("plantsoen", "")
                    .Replace("kamp", "")
                    .Replace("stee", "")
                    .Replace("gang", "")
                    .Replace("akker", "")
                    .Trim();
            newduo.Naam = naam;
            var huisnr = dice.Next(0, geoloc.NummerMax- geoloc.NummerMin) /2*2 + geoloc.NummerMin;//select an even number in range
            if (geoloc.NummerType == "odd")
                huisnr += 1;//set uneven when necessary
            huisnr = Math.Max(geoloc.NummerMin, Math.Min(geoloc.NummerMax, huisnr));
            
            newduo.Huisnummer = huisnr;
            
            Duos.Add(newduo);
            SaveChanges();
            return newduo;
        }

        public ICollection<Duo> SelectRandomDuos(int amount)
        {
            var duolist = Duos.ToList();

            if (amount > duolist.Count)
                amount = duolist.Count;

            var selected_duos = new List<Duo>();
            for (int j = 0; j < amount; j++)
            {
                var duo_to_add = duolist[dice.Next(duolist.Count)];
                while (selected_duos.Contains(duo_to_add))
                {
                    duo_to_add = duolist[dice.Next(duolist.Count)];
                }
                selected_duos.Add(duo_to_add);
            }
            return selected_duos;
        }

        public EventPlan CreateRandomPlan(ICollection<Duo> duos)
        {
            var stramienen = EventStramien.CreateMogelijkStramien(duos.Count);
            if (stramienen.Count == 0)
                return null;
            
            //leukste voor iedereen (voor nu?) -> maximum aantal groepsgrootte
            var gekozen_stramien = stramienen[0];

            var plan = new EventPlan()
            {
                Naam = "Grappige Naam voor een Eentje",
                AantalDeelnemers = duos.Count,
                //leukste voor iedereen (voor nu?) -> maximum aantal gangen
                AantalGangen = gekozen_stramien.MaxGangen,
                AantalGroepen = gekozen_stramien.Groepen,
                AantalDuosPerGroep = gekozen_stramien.Groepgrootte
            };

            foreach (var duo in duos)
            {
                plan.IngeschrevenDuos.Add(duo);
            }

            EventPlannen.Add(plan);
            SaveChanges();
            return plan;
        }

        private T[,] ArrayToMatrix<T>(List<T> items, int w, int h)
        {
            if (items.Count() != w * h)
                throw new InvalidOperationException();

            var matrix = new T[w, h];

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    matrix[x, y] = items[y * w + x];
                }
            }
            return matrix;
        }


        public EventSchema BruteForceSchemaSalesmanProblem(EventPlan plan)
        {
            var shortest_travel = double.MaxValue;
            EventSchema schema = null;
            for (int i = 0; i < 100; i++)
            {
                var new_schema = CreateSchemaFromPlan(plan);
                //avg traveldistance per duo per travel from meetup until last meal
                var travel_dist = CalculateSchemaTravelDistance(new_schema)/plan.AantalDeelnemers/plan.AantalGangen;
                if (travel_dist < shortest_travel)
                {
                    shortest_travel = travel_dist;
                    schema = new_schema;
                }
            }

            EventPlannen.Remove(plan);
            EventSchemas.Add(schema);
            SaveChanges();
            return schema;
        }

        private float CalculateSchemaTravelDistance(EventSchema schema, bool include_hometrip = false)
        {
            float distance = 0f;
            //praktijk:
            //ieder individu rijd van huis naar het verzamelpunt
            if (include_hometrip)
                foreach (var groep in schema.Gangen.First().Groepen)
                {
                    foreach (var gast in groep.Gasten)
                    {
                        distance += DistanceBetweenGeoLocations(gast.GeoLong, gast.GeoLat, schema.VerzamelLocatieLong, schema.VerzamelLocatieLat);
                    }
                }
            
            //ieder individu rijd naar zijn groeps-host (van het verzamelpunt)
            foreach (var groep in schema.Gangen.First().Groepen)
            {
                foreach (var gast in groep.Gasten)
                {
                    distance += DistanceBetweenGeoLocations(schema.VerzamelLocatieLong, schema.VerzamelLocatieLat, groep.Host.GeoLong, groep.Host.GeoLat);
                }
            }

            var gangen = schema.Gangen.OrderBy(g => g.StartTijd).ToList();
            //ieder individu rijd naar zijn groeps-host (van het vorige diner)
            if (include_hometrip)
                for (int g = 1; g < gangen.Count; g++)//skip gang 0
                {
                    foreach (var groep in gangen[g].Groepen)
                    {
                        foreach (var gast in groep.Gasten)
                        {
                            var gastgroep = FindDuoGroepInGang(gangen[g - 1], gast);
                            distance += DistanceBetweenGeoLocations(gastgroep.Host.GeoLong, gastgroep.Host.GeoLat, groep.Host.GeoLong, groep.Host.GeoLat);
                        }
                    }
                }
            
            //ieder individu rijd naar huis
            var laatste_gang = gangen[gangen.Count - 1];
            foreach (var groep in laatste_gang.Groepen)
            {
                foreach (var gast in groep.Gasten)
                {
                    distance += DistanceBetweenGeoLocations(groep.Host.GeoLong, groep.Host.GeoLat, gast.GeoLong, gast.GeoLat);
                }
            }

            return distance;
        }

        private static List<T> ShuffleList<T>(List<T> list)
        {
            Random rnd = new Random();
            for (int i = 0; i < list.Count; i++)
            {
                int k = rnd.Next(0, i);
                T value = list[k];
                list[k] = list[i];
                list[i] = value;
            }
            return list;
        }
        private EventSchema CreateSchemaFromPlan(EventPlan plan)
        {
            //TODO pak alle duos & random shuffle ze
            var duos = ShuffleList<Duo>( plan.IngeschrevenDuos.ToList() );
            
            //sanity-check which should never be triggered
            if (duos.Count != plan.AantalDeelnemers || plan.AantalGroepen * plan.AantalDuosPerGroep != plan.AantalDeelnemers)
                throw new InvalidOperationException();

            //TODO: implement plan-orders
            var geoloc = RandomGeoLocation();
            var result = new EventSchema()
            {
                VerzamelAdres = $"{geoloc.Straat} {geoloc.NummerMin}, {geoloc.Stad}",
                VerzamelDatum = DateTime.Now,
                VerzamelLocatieLong = geoloc.GeoLong,
                VerzamelLocatieLat = geoloc.GeoLat,
                VerzamelPostcode = geoloc.Postcode
            };

            var alle_groepen = new List<Groep>();

            //arrange duos in 2d-matrix van [AantalGroepen x AantalDuosPerGroep]
            var duos_2d = ArrayToMatrix<Duo>(duos, plan.AantalGroepen, plan.AantalDuosPerGroep);

            //gang 1:
            //elke groep bestaat uit verticale slices van de matrix 
            var gang1 = new Gang()
            {
                StartTijd = DateTime.Now,
                EindTijd = DateTime.Now
            };
            for (int g = 0; g < plan.AantalGroepen; g++)
            {
                var groep = new Groep();
                for (int d = 0; d < plan.AantalDuosPerGroep; d++)
                {
                    if (d==0)
                        groep.Host = duos_2d[g, d];
                    else
                        groep.Gasten.Add(duos_2d[g, d]);
                }
                alle_groepen.Add(groep);
                gang1.Groepen.Add(groep);
            }
            result.Gangen.Add(gang1);




            //gang 2:
            //elke groep bestaat uit diagonale slices van de matrix 
            //(groep 1 = [0,0],[1,1],[2,2],[3,3] groep 2 = [0,1],[1,2],[2,3],[3,4] tot aan AantalGroepen
            //  waarbij elke x-index met %AantalGroepen wrapped geselecteerd word)
            var gang2 = new Gang()
            {
                StartTijd = DateTime.Now,
                EindTijd = DateTime.Now
            };
            for (int g = 0; g < plan.AantalGroepen; g++)
            {
                var groep = new Groep();
                for (int d = 0; d < plan.AantalDuosPerGroep; d++)
                {
                    if (d == 1)
                        groep.Host = duos_2d[(g + d) % plan.AantalGroepen, d];
                    else
                        groep.Gasten.Add(duos_2d[(g + d) % plan.AantalGroepen, d]);
                }
                alle_groepen.Add(groep);
                gang2.Groepen.Add(groep);
            }
            result.Gangen.Add(gang2);


            if (plan.AantalGangen >= 3)
            {
                //gang 3:
                //elke groep bestaat uit diagonale slices van de matrix 
                //(groep 1 = [0,3],[1,2],[2,1],[3,0] groep 2 = [1,3],[2,2],[3,1],[4,0] tot aan AantalGroepen
                //  waarbij elke x-index met %AantalGroepen wrapped geselecteerd word)
                var gang3 = new Gang()
                {
                    StartTijd = DateTime.Now,
                    EindTijd = DateTime.Now
                };
                for (int g = 0; g < plan.AantalGroepen; g++)
                {
                    var groep = new Groep();
                    for (int d = 0; d < plan.AantalDuosPerGroep; d++)
                    {
                        if (d == 2)
                            groep.Host = duos_2d[(g - d + plan.AantalGroepen) % plan.AantalGroepen, d];
                        else
                            groep.Gasten.Add(duos_2d[(g - d + plan.AantalGroepen) % plan.AantalGroepen, d]);
                    }
                    alle_groepen.Add(groep);
                    gang3.Groepen.Add(groep);
                }
                result.Gangen.Add(gang3);

            }

            if (plan.AantalGangen >= 4)
            {
                //gang 4:
                //elke groep bestaat uit diagonale+2 slices van de matrix 
                //(groep 1 = [0,0],[2,1],[4,2],[6,3] groep 2 = [1,0],[3,1],[5,2],[7,3] tot aan AantalGroepen
                //  waarbij elke x-index met %AantalGroepen wrapped geselecteerd word)
                var gang4 = new Gang()
                {
                    StartTijd = DateTime.Now,
                    EindTijd = DateTime.Now
                };
                for (int g = 0; g < plan.AantalGroepen; g++)
                {
                    var groep = new Groep();
                    for (int d = 0; d < plan.AantalDuosPerGroep; d++)
                    {
                        if (d == 3)
                            groep.Host = duos_2d[(g + 2 * d) % plan.AantalGroepen, d];
                        else
                            groep.Gasten.Add(duos_2d[(g + 2 * d) % plan.AantalGroepen, d]);
                    }
                    alle_groepen.Add(groep);
                    gang4.Groepen.Add(groep);
                }
                result.Gangen.Add(gang4);

            }
            return result;
        }

        private float AnglesToRadians(float angle)
        {
            return (float) (angle / 180f * Math.PI);
        }
        private float DistanceBetweenGeoLocations(float long1, float lat1, float long2, float lat2) 
        {
            var long1_rad = AnglesToRadians(long1);
            var lat1_rad = AnglesToRadians(lat1);
            var long2_rad = AnglesToRadians(long2);
            var lat2_rad = AnglesToRadians(lat2);

            /*
            R = earth’s radius (mean radius = 6,371km)
            Δlong = long2− long1
            Δlat = lat2− lat1
            a = sin²(Δlat/2) + cos(lat1).cos(lat2).sin²(Δlong/2)
            c = 2.atan2(√a, √(1−a))
            d = R.c 
             */
            float r = 6_371_000f;
            var delta_long = long2_rad - long1_rad;
            var delta_lat = lat2_rad - lat1_rad;
            
            var a = Math.Pow(Math.Sin(delta_lat / 2), 2) + 
                    Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(delta_long / 2), 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            
            return (float)(r*c);
        }

        private Groep FindDuoGroepInGang(Gang gang, Duo duo)
        {
            var groepen = gang.Groepen.ToList();
            for (int g=0; g < groepen.Count; g++)
            {
                if (groepen[g].Host == duo ||
                    groepen[g].Gasten.Contains(duo))
                    return groepen[g];
            }
            throw new ArgumentException();
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
            for (int i = 1; i <= deelnemers_count / 2; i++)
            {
                if (deelnemers_count % i != 0) continue;

                var groep_count = i;
                var groep_grootte = deelnemers_count / i;


                if (groep_grootte < 2 || groep_grootte > 4)
                    continue;
                if (groep_count < 2 * groep_grootte)
                    continue;


                stramienen.Add(new EventStramien()
                {
                    MaxGangen = Math.Min(groep_grootte, Math.Min(groep_count, MAX_GANGEN)),
                    Groepen = groep_count,
                    Groepgrootte = groep_grootte
                });
            }
            return stramienen;
        }
    }

}