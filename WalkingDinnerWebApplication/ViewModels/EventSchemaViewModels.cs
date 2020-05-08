using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WalkingDinnerWebApplication.Models;

namespace WalkingDinnerWebApplication.ViewModels
{
    public class EventSchemaIndexViewModel
    {
        public IEnumerable<EventSchema> EventSchemas { get; set; }
    }


    public class EventSchemaViewModel
    {
        // Schema
        public int Id { get; set; }
        public string Naam { get; set; }
        public int AantalDeelnemers { get; set; }
        public int AantalGangen { get; set; }
        public int AantalGroepenPerGang { get; set; }
        public DateTime VerzamelDatum { get; set; }
        public string VerzamelAdres { get; set; }

        public ICollection<GroepenPerGang> GroepsverdelingPerGang { get; set; }

        // Duos
        public Dictionary<Duo, List<PathData>> DuoData { get; set; }
    }

    public class GroepenPerGang
    {
        public int GangNummer { get; set; }
        public ICollection<GangGroep> Groepen { get; set; }
    }

    public class GangGroep
    {
        public string Host { get; set; }
        public ICollection<string> Gasten { get; set; }
    }
}