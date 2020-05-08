using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WalkingDinnerWebApplication.DAL;
using WalkingDinnerWebApplication.Models;
using WalkingDinnerWebApplication.ViewModels;

namespace WalkingDinnerWebApplication.ModelMappingExtensions
{
    public static class EventSchemaExtensions
    {
        public static EventSchemaViewModel ToViewModel(this EventSchema model, WalkingDinnerContext db)
        {
            var viewModel = new EventSchemaViewModel
            {
                Id = model.Id,
                Naam = model.Naam,
                AantalDeelnemers = model.AantalDeelnemers,
                AantalGangen = model.AantalGangen,
                VerzamelDatum = model.VerzamelDatum,
                VerzamelAdres = model.VerzamelAdres,
                AantalGroepenPerGang = model.Gangen.First().Groepen.Count()
            };

            // Voegt de benodigde informatie toe om per gang alle groepen weer te geven
            viewModel.GroepsverdelingPerGang = new List<GroepenPerGang>();
            int gangNummer = 1;
            foreach (var g in model.Gangen)
            {
                var gang = new GroepenPerGang();
                gang.GangNummer = gangNummer;
                gang.Groepen = new List<GangGroep>();

                foreach (var gg in g.Groepen)
                {
                    var groep = new GangGroep();
                    groep.Host = gg.Host.Naam;
                    groep.Gasten = new List<string>();

                    foreach (var gast in gg.Gasten)
                    {
                        groep.Gasten.Add(gast.Naam);
                    }

                    gang.Groepen.Add(groep);
                }

                viewModel.GroepsverdelingPerGang.Add(gang);
                gangNummer++;
            }

            var seed = new DatabaseSeed(db);
            var pathing = seed.CalculateSchemaPathing(model);
            viewModel.DuoData = pathing;

            return viewModel;
        }
    }
}