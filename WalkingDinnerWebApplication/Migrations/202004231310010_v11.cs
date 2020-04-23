namespace WalkingDinnerWebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v11 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EventPlans", "AantalDeelnemers", c => c.Int(nullable: false));
            AddColumn("dbo.EventPlans", "AantalGangen", c => c.Int(nullable: false));
            AddColumn("dbo.EventPlans", "AantalGroepen", c => c.Int(nullable: false));
            AddColumn("dbo.EventPlans", "AantalDuosPerGroep", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EventPlans", "AantalDuosPerGroep");
            DropColumn("dbo.EventPlans", "AantalGroepen");
            DropColumn("dbo.EventPlans", "AantalGangen");
            DropColumn("dbo.EventPlans", "AantalDeelnemers");
        }
    }
}
