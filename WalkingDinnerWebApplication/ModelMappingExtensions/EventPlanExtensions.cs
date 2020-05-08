using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WalkingDinnerWebApplication.Models;
using WalkingDinnerWebApplication.ViewModels;

namespace WalkingDinnerWebApplication.ModelMappingExtensions
{
    public static class EventPlanExtensions
    {
        public static EventPlanViewModel ToViewModel(this EventPlan plan)
        {
            var viewModel = new EventPlanViewModel
            {
                Id = plan.Id,
                AantalDeelnemers = plan.AantalDeelnemers,
                AantalDuosPerGroep = plan.AantalDuosPerGroep,
                AantalGangen = plan.AantalGangen,
                AantalGroepen = plan.AantalGroepen,
                Naam = plan.Naam,
                IngeschrevenDuos = new List<DuoDetailsViewModel>()
            };

            foreach (var duo in plan.IngeschrevenDuos)
            {
                viewModel.IngeschrevenDuos.Add(new DuoDetailsViewModel()
                {
                    Adres = $"{duo.Straat} {duo.Huisnummer}",
                    Naam = duo.Naam,
                    Postcode = duo.PostCode,
                    Stad = duo.Stad
                });
            }

            return viewModel;
        }
    }
}