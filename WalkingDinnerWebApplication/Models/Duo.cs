using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WalkingDinnerWebApplication.Models
{
    public class Duo
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(6)]
        public string PostCode { get; set; }

        [Required]
        [MaxLength(64)]
        public string Adres { get; set; }

        [Required]
        public int Huisnummer { get; set; }

        [Required]
        public float GeoLong { get; set; }

        [Required]
        public float GeoLat { get; set; }

        [Required]
        public string Naam { get; set; }
        
        virtual public ICollection<EventPlan> IngeschrevenPlannen { get; set; } = new HashSet<EventPlan>();

        virtual public ICollection<Groep> GeplandeEventGroepen { get; set; } = new HashSet<Groep>();

    }
}
