using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WalkingDinnerWebApplication.Models
{
    public class EventSchema
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
        [MaxLength(64)]
        public string Naam { get; set; }

        [Required]
        public DateTime VerzamelDatum { get; set; }

        [Required]
        public float VerzamelLocatieLong { get; set; }

        [Required]
        public float VerzamelLocatieLat { get; set; }

        [Required]
        public string VerzamelAdres { get; set; }//string-weergave van straatnaam + huisnummer
        
        [Required]
        [MaxLength(6)]
        public string VerzamelPostcode { get; set; }

        virtual public ICollection<Gang> Gangen { get; set; } = new HashSet<Gang>();
    }
}
