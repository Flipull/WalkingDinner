using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WalkingDinnerWebApplication.Models
{
    [NotMapped]
    public class PathData
    {
        public float Long { get; set; }
        public float Lat { get; set; }
        public float Distance { get; set; }
    }
}