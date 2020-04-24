namespace WalkingDinnerWebApplication.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WalkingDinnerWebApplication.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<WalkingDinnerWebApplication.WalkingDinnerContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "WalkingDinnerWebApplication.WalkingDinnerContext";
        }

        protected override void Seed(WalkingDinnerContext context)
        {
            if (context.Duos.Count() > 0)
                return;
        }


    }



}
