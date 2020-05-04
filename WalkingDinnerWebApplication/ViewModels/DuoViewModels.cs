using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WalkingDinnerWebApplication.DAL;
using WalkingDinnerWebApplication.Models;

namespace WalkingDinnerWebApplication.ViewModels
{
    public class DuoDetailsViewModel
    {
        public string Naam { get; set; }
        public string Postcode { get; set; }
        public string Adres { get; set; }
        public string Stad { get; set; }

        public ICollection<EventPlan> EventPlannen { get; set; }
        public ICollection<EventSchema> EventSchemas { get; set; }
    }

    public class DuoEditViewModel
    {
        public string Naam { get; set; }
        public string Postcode { get; set; }
        public int Huisnummer { get; set; }
    }
}