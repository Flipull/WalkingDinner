using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using WalkingDinnerWebApplication.Models;
using WalkingDinnerWebApplication.ViewModels;

namespace WalkingDinnerWebApplication.DAL
{
    public class WalkingDinnerContext : DbContext
    {
        public WalkingDinnerContext() : base("WalkingDinnerConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Duo>()
                .HasMany<Groep>(d => d.GeplandeEventGroepen)
                .WithMany(g => g.Gasten);
            modelBuilder.Entity<Groep>()
                .HasRequired<Gang>(g => g.Gang)
                .WithMany(g => g.Groepen)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<Gang>()
                .HasRequired<EventSchema>(g => g.Schema)
                .WithMany(s => s.Gangen)
                .WillCascadeOnDelete(true);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Duo> Duos { get; set; }
        public DbSet<EventPlan> EventPlannen { get; set; }
        public DbSet<EventSchema> EventSchemas { get; set; }
        public DbSet<Gang> Gangen { get; set; }
        public DbSet<Groep> Groepen { get; set; }
        public DbSet<PostcodeGeoLocationCache> PostcodeGeoLocationCaches { get; set; }
    }
}