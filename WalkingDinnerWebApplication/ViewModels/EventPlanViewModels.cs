using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WalkingDinnerWebApplication.Models;

namespace WalkingDinnerWebApplication.ViewModels
{
    public class EventPlanIndexViewModel
    {
        public IEnumerable<EventPlan> EventPlans { get; set; }
    }
}