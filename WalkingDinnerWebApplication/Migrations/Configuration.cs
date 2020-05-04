namespace WalkingDinnerWebApplication.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WalkingDinnerWebApplication.DAL;
    using WalkingDinnerWebApplication.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<WalkingDinnerWebApplication.DAL.WalkingDinnerContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "WalkingDinnerWebApplication.DAL.WalkingDinnerContext";
        }

        protected override void Seed(WalkingDinnerContext context)
        {
            if (context.Duos.Count() > 0)
            {
                return;
            }
        }
    }
}
