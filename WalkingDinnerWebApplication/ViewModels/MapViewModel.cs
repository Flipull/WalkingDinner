using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WalkingDinnerWebApplication.Models;

namespace WalkingDinnerWebApplication.ViewModels
{
    public class PathData
    {
        public float Long { get; set; }
        public float Lat { get; set; }
        public float Distance { get; set; }
    }
    public class MapViewModel
    {
        //Schema-Data
        public int AantalDeelnemers { get; set; }
        public int AantalGangen { get; set; }
        public int AantalGroepen { get; set; }
        public int AantalDuosPerGroep { get; set; }
        public string Naam { get; set; }

        public DateTime VerzamelDatum { get; set; }
        public float VerzamelLocatieLong { get; set; }
        public float VerzamelLocatieLat { get; set; }
        public string VerzamelAdres { get; set; }//string-weergave van straatnaam + huisnummer
        public string VerzamelPostcode { get; set; }

        //Duo-Data
        public Dictionary<Duo, List<PathData>> DuoData { get; set; }

    }
}