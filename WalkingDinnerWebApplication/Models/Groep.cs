﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WalkingDinnerWebApplication.Models
{
    public class Groep
    {
        [Key]
        public int Id { get; set; }

        virtual public Gang Gang { get; set; }

        virtual public Duo Host { get; set; }
        
        virtual public ICollection<Duo> Gasten { get; set; } = new HashSet<Duo>();
        
    }
}
