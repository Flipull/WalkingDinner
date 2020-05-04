namespace WalkingDinnerWebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Duos", "Stad", c => c.String(nullable: false, maxLength: 32));
            AddColumn("dbo.Duos", "Straat", c => c.String(nullable: false, maxLength: 48));
            AddColumn("dbo.EventSchemas", "AantalDeelnemers", c => c.Int(nullable: false));
            AddColumn("dbo.EventSchemas", "AantalGangen", c => c.Int(nullable: false));
            AddColumn("dbo.EventSchemas", "AantalGroepen", c => c.Int(nullable: false));
            AddColumn("dbo.EventSchemas", "AantalDuosPerGroep", c => c.Int(nullable: false));
            AddColumn("dbo.EventSchemas", "Naam", c => c.String(nullable: false, maxLength: 64));
            AlterColumn("dbo.EventPlans", "Naam", c => c.String(nullable: false, maxLength: 64));
            CreateIndex("dbo.PostcodeGeoLocationCaches", "GeoLong");
            CreateIndex("dbo.PostcodeGeoLocationCaches", "GeoLat");
            DropColumn("dbo.Duos", "Adres");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Duos", "Adres", c => c.String(nullable: false, maxLength: 64));
            DropIndex("dbo.PostcodeGeoLocationCaches", new[] { "GeoLat" });
            DropIndex("dbo.PostcodeGeoLocationCaches", new[] { "GeoLong" });
            AlterColumn("dbo.EventPlans", "Naam", c => c.String(nullable: false));
            DropColumn("dbo.EventSchemas", "Naam");
            DropColumn("dbo.EventSchemas", "AantalDuosPerGroep");
            DropColumn("dbo.EventSchemas", "AantalGroepen");
            DropColumn("dbo.EventSchemas", "AantalGangen");
            DropColumn("dbo.EventSchemas", "AantalDeelnemers");
            DropColumn("dbo.Duos", "Straat");
            DropColumn("dbo.Duos", "Stad");
        }
    }
}
