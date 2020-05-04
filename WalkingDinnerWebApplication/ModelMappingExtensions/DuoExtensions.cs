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

                // TODO: Get all related EventSchemas from database
            };

            return viewModel;
        }

        public static DuoEditViewModel ToEditViewModel(this Duo model)
        {
            var viewModel = new DuoEditViewModel
            {
                Naam = model.Naam,
                Postcode = model.PostCode,
                Huisnummer = model.Huisnummer
            };

            return viewModel;
        }
    }
}