using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using WalkingDinnerWebApplication.Models;
using WalkingDinnerWebApplication.ViewModels;

namespace WalkingDinnerWebApplication
{
    public class WalkingDinnerContext : DbContext
    {
        private readonly int _CACHECOUNT;
        public WalkingDinnerContext() :
            base("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=WalkingDinner;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False")
        {
            _CACHECOUNT = this.Database.SqlQuery<int>("select count(*) from PostcodeGeoLocationCaches").First();
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
                loc = PostcodeGeoLocationCaches.OrderBy(p => p.Id)
                                                .Skip(dice.Next(0, _CACHECOUNT))
                                                .Take(1).First();
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
            
            var gekozen_stramien = stramienen[Math.Max(0 ,dice.Next(0,stramienen.Count+1)-1)];
            
            var plan = new EventPlan()
            {
                Naam = "Grappige Naam voor een Eentje",
                AantalDeelnemers = duos.Count,
                AantalGangen = dice.Next(EventStramien.MIN_GANGEN,gekozen_stramien.MaxGangen+1),
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

        private T[,] ArrayToMatrixTransposed<T>(List<T> items, int w, int h)
        {
            if (items.Count() != w * h)
                throw new InvalidOperationException();

            var matrix = new T[w, h];

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    matrix[x, y] = items[y  + x * h];
                }
            }
            return matrix;
        }

        private T[,] ArrayToMatrix<T>(List<T> items, int w, int h)
        {
            if (items.Count() != w * h)
                throw new InvalidOperationException();

            var matrix = new T[w, h];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    matrix[x, y] = items[y * w + x];
                }
            }
            return matrix;
        }

        public EventSchema BruteForceSchemaSalesmanProblem(EventPlan plan)
        {
            var shortest_travel = float.MaxValue;
            EventSchema schema = null;
            for (int i = 0; i < 250; i++)
            {
                var new_schema = CreateSchemaFromPlan(plan);
                
                var paths = CalculateSchemaPathing(new_schema);
                var travel_dist = CalculatePathMapTravelDistance(paths);

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

        public Dictionary<Duo, List<PathData>> CalculateSchemaPathing(EventSchema schema)
        {
            var paths = new Dictionary<Duo, List<PathData>>();

            var gangen = schema.Gangen.OrderBy(g => g.Id).ToList();

            //create empty routes/paths, declaring home-location and 0 travel-time/distance
            foreach (var groep in gangen[0].Groepen)
            {
                var duopathdata = new PathData()
                {
                    Long = groep.Host.GeoLong,
                    Lat = groep.Host.GeoLat,
                    Distance = 0
                };
                paths.Add(groep.Host, new List<PathData>() { duopathdata });

                foreach (var gast in groep.Gasten)
                {
                    duopathdata = new PathData()
                    {
                        Long = gast.GeoLong,
                        Lat = gast.GeoLat,
                        Distance = 0
                    };
                    paths.Add(gast, new List<PathData>() { duopathdata } );
                }
            }


            //praktijk:

            /*
            //ieder individu rijd van huis naar de eerste host
            foreach (var groep in schema.Gangen.First().Groepen)
            {
                paths[groep.Host].Add(
                    new PathData()
                    {
                        Long = groep.Host.GeoLong,
                        Lat = groep.Host.GeoLat,
                        Distance = 0
                    });
                foreach (var gast in groep.Gasten)
                {
                    var distance = DistanceBetweenGeoLocations(gast.GeoLong, gast.GeoLat, groep.Host.GeoLong, groep.Host.GeoLat);
                    paths[gast].Add(
                        new PathData()
                        {
                            Long = groep.Host.GeoLong,
                            Lat = groep.Host.GeoLat,
                            Distance = distance
                        });
                }
            }
            */


            //ieder individu rijd van huis naar het verzamelpunt
            foreach (var groep in gangen[0].Groepen)
            {
                var distance = DistanceBetweenGeoLocations(groep.Host.GeoLong, groep.Host.GeoLat, schema.VerzamelLocatieLong, schema.VerzamelLocatieLat);
                paths[groep.Host].Add(new PathData()
                {
                    Long = schema.VerzamelLocatieLong,
                    Lat = schema.VerzamelLocatieLat,
                    Distance = distance
                });
                foreach (var gast in groep.Gasten)
                {
                    distance = DistanceBetweenGeoLocations(gast.GeoLong, gast.GeoLat, schema.VerzamelLocatieLong, schema.VerzamelLocatieLat);
                    paths[gast].Add(
                        new PathData()
                        {
                            Long = schema.VerzamelLocatieLong,
                            Lat = schema.VerzamelLocatieLat,
                            Distance = distance
                        });
                }
            }
            //ieder individu rijd naar zijn groeps-host (van het verzamelpunt)
            foreach (var groep in gangen[0].Groepen)
            {
                var distance = DistanceBetweenGeoLocations(schema.VerzamelLocatieLong, schema.VerzamelLocatieLat, groep.Host.GeoLong, groep.Host.GeoLat);
                paths[groep.Host].Add(new PathData()
                {
                    Long = groep.Host.GeoLong,
                    Lat = groep.Host.GeoLat,
                    Distance = distance
                });
                foreach (var gast in groep.Gasten)
                {
                    distance = DistanceBetweenGeoLocations(schema.VerzamelLocatieLong, schema.VerzamelLocatieLat, groep.Host.GeoLong, groep.Host.GeoLat);
                    paths[gast].Add(
                        new PathData()
                        {
                            Long = groep.Host.GeoLong,
                            Lat = groep.Host.GeoLat,
                            Distance = distance
                        });
                }
            }


            //ieder individu rijd naar zijn groeps-host (van het vorige diner)
            for (int g = 1; g < gangen.Count; g++)//skip gang 0
            {
                foreach (var groep in gangen[g].Groepen)
                {
                    var gastgroep = FindDuoGroepInGang(gangen[g - 1], groep.Host);
                    if (gastgroep == null)
                    {
                        PrintSchema(schema);
                        throw new ArgumentException($"Duo #{groep.Host.Id} not found in schema in gang {g}");
                    }
                    var distance = DistanceBetweenGeoLocations(gastgroep.Host.GeoLong, gastgroep.Host.GeoLat, groep.Host.GeoLong, groep.Host.GeoLat);
                    paths[groep.Host].Add(new PathData()
                    {
                        Long = groep.Host.GeoLong,
                        Lat = groep.Host.GeoLat,
                        Distance = distance
                    });
                    foreach (var gast in groep.Gasten)
                    {
                        gastgroep = FindDuoGroepInGang(gangen[g - 1], gast);
                        if (gastgroep == null)
                        {
                            PrintSchema(schema);
                            throw new ArgumentException($"Duo #{gast.Id} not found in schema in gang {g}");
                        }
                        distance = DistanceBetweenGeoLocations(gastgroep.Host.GeoLong, gastgroep.Host.GeoLat, groep.Host.GeoLong, groep.Host.GeoLat);
                        paths[gast].Add(
                            new PathData()
                            {
                                Long = groep.Host.GeoLong,
                                Lat = groep.Host.GeoLat,
                                Distance = distance
                            });
                    }
                }
            }
            
            //ieder individu rijd naar huis
            var laatste_gang = gangen[gangen.Count - 1];
            foreach (var groep in laatste_gang.Groepen)
            {
                //last host goes home => always zero distance
                var distance = DistanceBetweenGeoLocations(groep.Host.GeoLong, groep.Host.GeoLat, groep.Host.GeoLong, groep.Host.GeoLat);
                Debug.WriteLine(distance);
                paths[groep.Host].Add(new PathData()
                {
                    Long = groep.Host.GeoLong,
                    Lat = groep.Host.GeoLat,
                    Distance = distance
                });
                foreach (var gast in groep.Gasten)
                {
                    distance = DistanceBetweenGeoLocations(groep.Host.GeoLong, groep.Host.GeoLat, gast.GeoLong, gast.GeoLat);
                    paths[gast].Add(
                        new PathData()
                        {
                            Long = gast.GeoLong,
                            Lat = gast.GeoLat,
                            Distance = distance
                        });
                }
            }

            return paths;
        }
        public float CalculatePathMapTravelDistance(Dictionary<Duo, List<PathData>> pathmap)
        {
            double distance = 0;
            //voor elke duo, bereken de afstanden van zijn path
            foreach(var item in pathmap)
            {
                //squared-distance(weight) == distance weighs heavier; evens out distances between duos

                var travel_items = item.Value;//.GetRange(1, item.Value.Count-2);
                distance += Math.Pow(travel_items.Sum(p => p.Distance),2);
                if (double.IsNaN(distance) )
                {
                    Console.WriteLine("!!!");
                }
            }
            return (float)distance;
        }


        private static List<T> CircularList<T>(List<T> list)
        {
            var count = list.Count;
            int returncircle_start = (count % 2 == 1 ? 1 : 0);
            List<T> result = new List<T>();
            
            for (int i = count-1; i >= 0; i = i - 2)
            {
                result.Add(list[i]);
            }
            for (int i = returncircle_start; i < count; i = i + 2)
            {
                result.Add(list[i]);
            }
            return result;

        }
        private static List<T> ShuffleList<T>(List<T> list, int idx_low, int idx_hi, int count)
        {
            //in-place shuffle
            idx_low = Math.Max(0, Math.Min(list.Count, idx_low));
            idx_hi = Math.Min(list.Count, Math.Max(idx_hi, idx_low));
            count = Math.Min(count, idx_hi - idx_low);

            Random rnd = new Random();
            for (int i = 0; i < count; i++)
            {
                int k = rnd.Next(idx_low, idx_hi);
                int l = rnd.Next(idx_low, idx_hi);
                T value = list[k];
                list[k] = list[l];
                list[l] = value;
            }
            return list;
        }

        private EventSchema CreateSchemaFromPlan(EventPlan plan)
        {
            //sanity-check which should never be triggered
            if (plan.IngeschrevenDuos.Count != plan.AantalDeelnemers || plan.AantalGroepen * plan.AantalDuosPerGroep != plan.AantalDeelnemers)
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


            //TODO pak alle duos & random shuffle ze
            List<Duo> duos = plan.IngeschrevenDuos
                                .OrderBy(d => DistanceBetweenGeoLocations(result.VerzamelLocatieLong, result.VerzamelLocatieLat,
                                                                          d.GeoLong, d.GeoLat
                    )
                                )
                                .ToList();

            //duos = CircularList<Duo>(duos);
            duos = ShuffleList<Duo>(duos, 0, duos.Count / plan.AantalDuosPerGroep*2, dice.Next(duos.Count / plan.AantalDuosPerGroep));
            duos = ShuffleList<Duo>(duos, dice.Next(duos.Count), duos.Count, dice.Next(duos.Count));
            
            //var duos = ShuffleList<Duo>(plan.IngeschrevenDuos.ToList(), 0, plan.IngeschrevenDuos.Count, plan.IngeschrevenDuos.Count);

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

                //gang 4:
                //elke groep bestaat uit een mixed set van de matrix 
                //(groep 1 = [1,0],[3,1],[0,2],[2,3] groep 2 = [2,0],[4,1],[1,2],[3,3] tot aan AantalGroepen
                //  waarbij elke x-index met %AantalGroepen wrapped geselecteerd word)
                var gang4 = new Gang()
                {
                    StartTijd = DateTime.Now,
                    EindTijd = DateTime.Now
                };
                for (int g = 0; g < plan.AantalGroepen; g++)
                {
                    var groep = new Groep();
                    groep.Gasten.Add(duos_2d[(g) % plan.AantalGroepen, 0]);
                    groep.Gasten.Add(duos_2d[(g + 2) % plan.AantalGroepen, 1]);
                    groep.Gasten.Add(duos_2d[(g - 1 + plan.AantalGroepen) % plan.AantalGroepen, 2]);
                    groep.Host = duos_2d[(g + 1) % plan.AantalGroepen, 3];
                    /*
                    for (int d = 0; d < plan.AantalDuosPerGroep; d++)
                    {
                        if (d == 3)
                            groep.Host = duos_2d[(g + 2 * d) % plan.AantalGroepen, d];
                        else
                            groep.Gasten.Add(duos_2d[(g + 2 * d) % plan.AantalGroepen, d]);
                    }
                    */
                    alle_groepen.Add(groep);
                    gang4.Groepen.Add(groep);
                }
                result.Gangen.Add(gang4);

            }
            return result;
        }

        private void PrintSchema(EventSchema schema)
        {
            foreach(var gang in schema.Gangen)
            {
                System.Diagnostics.Debug.WriteLine("---");
                foreach (var groep in gang.Groepen)
                {
                    var str = groep.Host.Id.ToString().PadLeft(5);
                    foreach (var duo in groep.Gasten)
                    {
                        str += duo.Id.ToString().PadLeft(5);
                    }
                    System.Diagnostics.Debug.WriteLine(str);
                }
            }
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
                    Math.Cos(lat1_rad) * Math.Cos(lat2_rad) * Math.Pow(Math.Sin(delta_long / 2), 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            
            var result = (float)(r*c);
            if (float.IsNaN(result)) throw new DivideByZeroException();
            return result;
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