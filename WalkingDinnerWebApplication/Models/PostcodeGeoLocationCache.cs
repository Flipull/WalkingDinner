using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WalkingDinnerWebApplication.Models
{
    public class PostcodeGeoLocationCache
    {
        [Key]
        public int Id { get; set; }

        [Index]
        [MaxLength(6)]
        public string Postcode { get; set; }

        [Required]
        public int NummerMin { get; set; }
        [Required]
        public int NummerMax { get; set; }
        
        [MaxLength(5)]//nummerType is null -> Postcode gereserveert voor PostbusAdressen
        public string NummerType { get; set; }

        [Index]
        [Required]
        public float GeoLong { get; set; }

        [Index]
        [Required]
        public float GeoLat { get; set; }

        [Required]
        public float rd_x { get; set; }

        [Required]
        public float rd_y { get; set; }
        
        
        [Required]
        [MaxLength(32)]
        public string Stad { get; set; }

        [Required]
        [MaxLength(48)]
        public string Straat { get; set; }
    }
}