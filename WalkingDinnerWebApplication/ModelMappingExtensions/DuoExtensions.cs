using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WalkingDinnerWebApplication.DAL;
using WalkingDinnerWebApplication.Models;
using WalkingDinnerWebApplication.ViewModels;

namespace WalkingDinnerWebApplication.ModelMappingExtensions
{
    public static class DuoExtensions
    {
        public static DuoDetailsViewModel ToDetailsViewModel(this Duo model, WalkingDinnerContext db)
        {
            var viewModel = new DuoDetailsViewModel
            {
                Naam = model.Naam,
                Postcode = model.PostCode,
                Adres = $"{model.Straat} {model.Huisnummer}",
                Stad = model.Stad,
                EventPlannen = model.IngeschrevenPlannen
            };

            // Get EventSchemas the duo is attending
            viewModel.EventSchemas = new List<EventSchema>();
            foreach(var gang in db.Gangen)
            {
                bool gangContainsDuo = false;

                foreach(var groep in gang.Groepen)
                {
                    if (groep.Gasten.Contains(model))
                    {
                        gangContainsDuo = true;
                    }
                }

                if (gangContainsDuo)
                {
                    viewModel.EventSchemas.Add(gang.Schema);
                }
            }

            return viewModel;
        }
    }
}