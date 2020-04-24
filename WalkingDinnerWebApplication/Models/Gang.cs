using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WalkingDinnerWebApplication.Models
{
    public class Gang
    {
        [Key]
        public int Id { get; set; }

        public DateTime StartTijd { get; set; }
        public DateTime EindTijd { get; set; }


        virtual public EventSchema Schema { get; set; }
        virtual public ICollection<Groep> Groepen { get; set; } = new HashSet<Groep>();

    }
}
