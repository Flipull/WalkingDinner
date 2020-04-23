using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WalkingDinnerWebApplication.Models
{
    class DuoEventPlan
    {
        [Key]
        public int DuoId { get; set; }
        [Key]
        public int EventPlanId { get; set; }

        virtual public Duo Duo { get; set; }
        virtual public EventPlan EventPlan { get; set; }
    }
}
