﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WalkingDinnerLibrary
{
    public class EventPlan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AantalDeelnemers { get; set; }
        [Required]
        public int AantalGangen { get; set; }
        [Required]
        public int AantalGroepen { get; set; }
        [Required]
        public int AantalDuosPerGroep { get; set; }
        

        [Required]
        public string Naam { get; set; }

        
        virtual public ICollection<Duo> IngeschrevenDuos { get; set; }

    }
}
