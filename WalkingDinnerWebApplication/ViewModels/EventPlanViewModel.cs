using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WalkingDinnerWebApplication.ViewModels
{
    public class EventPlanViewModel
    {
        public int Id { get; set; }
        public int AantalDeelnemers { get; set; }
        public int AantalGangen { get; set; }
        public int AantalGroepen { get; set; }
        public int AantalDuosPerGroep { get; set; }
        public string Naam { get; set; }
        public List<DuoDetailsViewModel> IngeschrevenDuos { get; set; }
        
    }
}