using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WalkingDinnerLibrary
{
    public class EventSchema
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime VerzamelDatum { get; set; }

        [Required]
        public double VerzamelLocatieLong { get; set; }

        [Required]
        public double VerzamelLocatieLat { get; set; }

        [Required]
        public string VerzamelAdres { get; set; }//string-weergave van straatnaam + huisnummer
        
        [Required]
        [MaxLength(6)]
        public string VerzamelPostcode { get; set; }

        virtual public ICollection<Gang> Gangen { get; set; }

    }
}
