using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WalkingDinnerLibrary
{
    public class Duo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(6)]
        public string PostCode { get; set; }

        [Required]
        [MaxLength(8)]
        public string Huisnummer { get; set; }

        [Required]
        public double GeoLong { get; set; }

        [Required]
        public double GeoLat { get; set; }

        [Required]
        public string Naam { get; set; }

        virtual public ICollection<EventPlan> IngeschrevenPlannen {get; set;}
        
    }
}
