using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WalkingDinnerLibrary;

namespace WalkingDinnerWebApplication
{
    public class WalkingDinnerContext : DbContext
    {
        public WalkingDinnerContext(): 
            base("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False")
        {
            
        }

        public DbSet<Duo> Duos { get; set; }
        public DbSet<EventPlan> EventPlannen { get; set; }
        public DbSet<EventSchema> EventSchemas { get; set; }
        public DbSet<Gang> Gangen { get; set; }
        public DbSet<Groep> Groepen { get; set; }
    }
}