using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WalkingDinnerWebApplication.Models;
using WalkingDinnerWebApplication.ViewModels;

namespace WalkingDinnerWebApplication.DAL
{
    public class DatabaseSeed
    {
        public DatabaseSeed(WalkingDinnerContext context)
        {
            db = context;
        }

        private WalkingDinnerContext db;

        private Random dice = new Random();
        private int _CACHECOUNT = 0;
        public PostcodeGeoLocationCache RandomGeoLocation()
        {
            if (_CACHECOUNT == 0)
                _CACHECOUNT = db.Database.SqlQuery<int>("select count(*) from PostcodeGeoLocationCaches").First();
            //TODO: stop hanging when you forgot to fill the database-table
            PostcodeGeoLocationCache loc = null;
            loc = db.PostcodeGeoLocationCaches.OrderBy(p => p.Id)
                                            .Skip(dice.Next(0, _CACHECOUNT))
                                            .Take(1).First();
            return loc;
        }
        public PostcodeGeoLocationCache PostcodeToGeoLocation(string postcode, int home_nr)
        {
            //if cached data exists, return it
            var caches = db.PostcodeGeoLocationCaches
                    .Where(o => o.Postcode == postcode.ToUpper() &&
                                o.NummerMin <= home_nr && o.NummerMax >= home_nr &&
                                (
                                    (o.NummerType == "mixed") ||
                                    (o.NummerType == "even" && home_nr % 2 == 0) ||
                                    (o.NummerType == "odd" && home_nr % 2 == 1)
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
            newduo.Stad = geoloc.Stad;
            newduo.Straat = geoloc.Straat;
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
            var huisnr = dice.Next(0, geoloc.NummerMax - geoloc.NummerMin) / 2 * 2 + geoloc.NummerMin;//select an even number in range
            if (geoloc.NummerType == "odd")
                huisnr += 1;//set uneven when necessary
            huisnr = Math.Max(geoloc.NummerMin, Math.Min(geoloc.NummerMax, huisnr));

            newduo.Huisnummer = huisnr;

            db.Duos.Add(newduo);
            db.SaveChanges();
            return newduo;
        }

        public ICollection<Duo> SelectRandomDuos(int amount)
        {
            var duolist = db.Duos.ToList();

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

        /// <summary>
        /// new EventPlan (autosaved to Db!)
        /// </summary>
        /// <param name="aantal_duos"></param>
        /// <param name="aantal_groepen"></param>
        /// <param name="aantal_duospergroep"></param>
        /// <param name="aantal_gangen"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public EventPlan CreatePlan(int aantal_duos, int aantal_duospergroep, int aantal_gangen, string name)
        {
            //TODO: validate data
            if (aantal_duos % aantal_duospergroep != 0)
                throw new InvalidOperationException();


            var plan = new EventPlan()
            {
                Naam = name,
                AantalDeelnemers = aantal_duos,
                AantalGangen = aantal_gangen,
                AantalGroepen = aantal_duos / aantal_duospergroep,
                AantalDuosPerGroep = aantal_duospergroep
            };
            db.EventPlannen.Add(plan);
            db.SaveChanges();
            return plan;
        }

        public EventPlan CreateRandomPlan(ICollection<Duo> duos)
        {
            var stramienen = EventStramien.CreateMogelijkStramien(duos.Count);
            if (stramienen.Count == 0)
                return null;

            var gekozen_stramien = stramienen[Math.Max(0, dice.Next(0, stramienen.Count + 1) - 1)];

            var plan = new EventPlan()
            {
                Naam = "Grappige Naam voor een Eventje",
                AantalDeelnemers = duos.Count,
                AantalGangen = dice.Next(EventStramien.MIN_GANGEN, gekozen_stramien.MaxGangen + 1),
                AantalGroepen = gekozen_stramien.Groepen,
                AantalDuosPerGroep = gekozen_stramien.Groepgrootte
            };

            foreach (var duo in duos)
            {
                plan.IngeschrevenDuos.Add(duo);
            }

            db.EventPlannen.Add(plan);
            db.SaveChanges();
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
                    matrix[x, y] = items[y + x * h];
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

        private PostcodeGeoLocationCache GeoLocationToCache(float lng, float lat)
        {
            PostcodeGeoLocationCache any_cache = null;
            /*
            any_cache = PostcodeGeoLocationCaches
                .OrderBy(c => Math.Abs(lng - c.GeoLong) + Math.Abs(lat - c.GeoLat))
                .FirstOrDefault();
            */
            //optimized version for DB-indices
            float epsilon = 1 / 2048f;
            do
            {
                epsilon *= 2f;
                any_cache = db.PostcodeGeoLocationCaches
                    .Where(c => c.GeoLong > lng - epsilon && c.GeoLong < lng + epsilon
                             && c.GeoLat > lat - epsilon && c.GeoLat < lat + epsilon)
                    .FirstOrDefault();
            } while (any_cache == null);
            return any_cache;
        }

        private PostcodeGeoLocationCache DuosToAverageLocation(List<Duo> duos)
        {
            var total = duos.Count;
            float long_sum = 0;
            float lat_sum = 0;
            foreach (var duo in duos)
            {
                long_sum += duo.GeoLong;
                lat_sum += duo.GeoLat;
            }
            float long_avg = long_sum / total;
            float lat_avg = lat_sum / total;

            PostcodeGeoLocationCache any_cache = null;
            float epsilon = 2e-6f;
            do
            {
                epsilon *= 1.5f;
                any_cache = db.PostcodeGeoLocationCaches
                    .Where(c => c.GeoLong > long_avg - epsilon && c.GeoLong < long_avg + epsilon
                            && c.GeoLat > lat_avg - epsilon && c.GeoLat < lat_avg + epsilon)
                    .FirstOrDefault();
            } while (any_cache == null);
            return any_cache;
        }

        public EventSchema BruteForceSchemaSalesmanProblem(EventPlan plan)
        {
            var epsilon = 1.00;
            //var meetingloc = DuosToAverageLocation(plan.IngeschrevenDuos.ToList());

            var duos = plan.IngeschrevenDuos.ToList();
            var long_min = duos.Min(d => d.GeoLong);
            var long_max = duos.Max(d => d.GeoLong);
            var lat_min = duos.Min(d => d.GeoLat);
            var lat_max = duos.Max(d => d.GeoLat);

            float rand_long = (float)dice.NextDouble() * (long_max - long_min) + long_min;
            float rand_lat = (float)dice.NextDouble() * (lat_max - lat_min) + lat_min;
            var meetingloc = GeoLocationToCache(rand_long, rand_lat);

            var shortest_travel = float.MaxValue;
            EventSchema schema = null;
            for (int i = 0; i < 250; i++)
            {
                var new_schema = CreateSchemaFromPlan(plan, meetingloc);
                var paths = CalculateSchemaPathing(new_schema);
                


                var travel_dist = CalculatePathMapSquaredDistance(paths);
                
                if (travel_dist < shortest_travel)
                {
                    shortest_travel = travel_dist;
                    schema = new_schema;
                    epsilon += 0.05;
                }
                else if (travel_dist / shortest_travel > epsilon)//threshold to change meetingloc
                {
                    //meetingloc = RandomGeoLocation();
                    rand_long = (float)dice.NextDouble() * (long_max - long_min) + long_min;
                    rand_lat = (float)dice.NextDouble() * (lat_max - lat_min) + lat_min;
                    meetingloc = GeoLocationToCache(rand_long, rand_lat);
                    epsilon += 0.01;
                }
                else
                {
                    epsilon = Math.Max(1, epsilon - 0.01);
                    //System.Diagnostics.Debug.WriteLine(travel_dist / shortest_travel);
                }
            }

            db.EventPlannen.Remove(plan);
            db.EventSchemas.Add(schema);
            db.SaveChanges();
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
                    paths.Add(gast, new List<PathData>() { duopathdata });
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
        public float CalculatePathMapSquaredDistance(Dictionary<Duo, List<PathData>> pathmap)
        {
            double distance = 0;
            //voor elke duo, bereken de afstanden van zijn path
            foreach (var item in pathmap)
            {
                //squared-distance(weight) == distance weighs heavier; evens out distances between duos

                var travel_items = item.Value;//.GetRange(1, item.Value.Count-2);
                distance += Math.Pow(travel_items.Sum(p => p.Distance), 2);
                if (double.IsNaN(distance))
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

            for (int i = count - 1; i >= 0; i = i - 2)
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


        private static int EXPECTED_TRAVELSPEEDS = 50_000;//meter per hour
        private static float ROUND_TRESHOLD = 5/60f;//minutes precision on clock (in hours)
        
        private DateTime AddRoundedTime(DateTime input, float add_hours)
        {
            var addition = Math.Round(add_hours / ROUND_TRESHOLD + ROUND_TRESHOLD) * ROUND_TRESHOLD;
            return input.AddHours(addition);
        }
        private EventSchema CreateSchemaFromPlan(EventPlan plan, PostcodeGeoLocationCache geoloc)
        {
            //sanity-check which should never be triggered
            if (plan.IngeschrevenDuos.Count != plan.AantalDeelnemers || plan.AantalGroepen * plan.AantalDuosPerGroep != plan.AantalDeelnemers)
                throw new InvalidOperationException();

            //TODO: implement custom plan-orders
            var result = new EventSchema()
            {
                AantalDeelnemers = plan.AantalDeelnemers,
                AantalGangen = plan.AantalGangen,
                AantalGroepen = plan.AantalGroepen,
                AantalDuosPerGroep = plan.AantalDuosPerGroep,
                Naam = plan.Naam,

                VerzamelAdres = $"{geoloc.Straat} {geoloc.NummerMin}, {geoloc.Stad}",
                //datum: altijd, morgen-avond 18 uur
                VerzamelDatum = DateTime.Now.AddDays(1).Date.AddHours(18),
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
            duos = ShuffleList<Duo>(duos, 0, plan.AantalGroepen, dice.Next(plan.AantalGroepen));
            duos = ShuffleList<Duo>(duos, plan.AantalGroepen, plan.AantalGroepen * 2, dice.Next(plan.AantalGroepen));
            if (plan.AantalDuosPerGroep >= 3)
                duos = ShuffleList<Duo>(duos, plan.AantalGroepen * plan.AantalGangen, plan.AantalGroepen * plan.AantalDuosPerGroep,
                                        dice.Next(plan.AantalGroepen * (plan.AantalDuosPerGroep - plan.AantalGangen))
                        );

            //duos = ShuffleList<Duo>(duos, dice.Next(duos.Count), duos.Count, dice.Next(duos.Count));

            //var duos = ShuffleList<Duo>(plan.IngeschrevenDuos.ToList(), 0, plan.IngeschrevenDuos.Count, plan.IngeschrevenDuos.Count);

            //arrange duos in 2d-matrix van [AantalGroepen x AantalDuosPerGroep]
            var duos_2d = ArrayToMatrix<Duo>(duos, plan.AantalGroepen, plan.AantalDuosPerGroep);

            //gang 1:
            //elke groep bestaat uit verticale slices van de matrix 
            var gang1 = new Gang()
            {
            };
            for (int g = 0; g < plan.AantalGroepen; g++)
            {
                var groep = new Groep();
                for (int d = 0; d < plan.AantalDuosPerGroep; d++)
                {
                    if (d == 0)
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


            var gang3 = new Gang()
            {
            };
            if (plan.AantalGangen >= 3)
            {
                //gang 3:
                //elke groep bestaat uit diagonale slices van de matrix 
                //(groep 1 = [0,3],[1,2],[2,1],[3,0] groep 2 = [1,3],[2,2],[3,1],[4,0] tot aan AantalGroepen
                //  waarbij elke x-index met %AantalGroepen wrapped geselecteerd word)
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

            var gang4 = new Gang()
            {
            };
            if (plan.AantalGangen >= 4)
            {
                //gang 4:
                //elke groep bestaat uit diagonale+2 slices van de matrix 
                //(groep 1 = [0,0],[2,1],[4,2],[6,3] groep 2 = [1,0],[3,1],[5,2],[7,3] tot aan AantalGroepen

                //gang 4:
                //elke groep bestaat uit een mixed set van de matrix 
                //(groep 1 = [1,0],[3,1],[0,2],[2,3] groep 2 = [2,0],[4,1],[1,2],[3,3] tot aan AantalGroepen
                //  waarbij elke x-index met %AantalGroepen wrapped geselecteerd word)
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



            var pathing = CalculateSchemaPathing(result);
            var paths_max_distance = new float[plan.AantalGangen];

            //pathing= [[home],[meetpoint], [g1], [g2], [g3],  [g4], [home] ]


            for (var i = 0; i < plan.AantalGangen; i++)
            {
                paths_max_distance[i] = pathing.Max(a => a.Value[i+2].Distance);
            }

            gang1.StartTijd = AddRoundedTime(result.VerzamelDatum, paths_max_distance[0] / EXPECTED_TRAVELSPEEDS);
            gang1.EindTijd = gang1.StartTijd.AddHours(0.25);

            gang2.StartTijd = AddRoundedTime(gang1.EindTijd, paths_max_distance[1] / EXPECTED_TRAVELSPEEDS);
            gang2.EindTijd = gang2.StartTijd.AddHours(0.5);
            
            if (plan.AantalGangen >= 3)
            {
                gang3.StartTijd = AddRoundedTime(gang2.EindTijd, paths_max_distance[2] / EXPECTED_TRAVELSPEEDS);
                gang3.EindTijd = gang3.StartTijd.AddHours(1);
            }
            
            if (plan.AantalGangen >= 4)
            {
                gang4.StartTijd = AddRoundedTime(gang3.EindTijd, paths_max_distance[3] / EXPECTED_TRAVELSPEEDS);
                gang4.EindTijd = gang4.StartTijd.AddHours(0.5);
            }

            return result;
        }

        private void PrintSchema(EventSchema schema)
        {
            foreach (var gang in schema.Gangen)
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
            return (float)(angle / 180f * Math.PI);
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

            var result = (float)(r * c);
            if (float.IsNaN(result)) throw new DivideByZeroException();
            return result;
        }

        private Groep FindDuoGroepInGang(Gang gang, Duo duo)
        {
            var groepen = gang.Groepen.ToList();
            for (int g = 0; g < groepen.Count; g++)
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