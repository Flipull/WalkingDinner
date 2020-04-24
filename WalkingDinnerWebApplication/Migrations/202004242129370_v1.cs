namespace WalkingDinnerWebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Duos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PostCode = c.String(nullable: false, maxLength: 6),
                        Adres = c.String(nullable: false, maxLength: 64),
                        Huisnummer = c.Int(nullable: false),
                        GeoLong = c.Single(nullable: false),
                        GeoLat = c.Single(nullable: false),
                        Naam = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Groeps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Gang_Id = c.Int(nullable: false),
                        Host_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Gangs", t => t.Gang_Id, cascadeDelete: true)
                .ForeignKey("dbo.Duos", t => t.Host_Id)
                .Index(t => t.Gang_Id)
                .Index(t => t.Host_Id);
            
            CreateTable(
                "dbo.Gangs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartTijd = c.DateTime(nullable: false),
                        EindTijd = c.DateTime(nullable: false),
                        Schema_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EventSchemas", t => t.Schema_Id, cascadeDelete: true)
                .Index(t => t.Schema_Id);
            
            CreateTable(
                "dbo.EventSchemas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VerzamelDatum = c.DateTime(nullable: false),
                        VerzamelLocatieLong = c.Single(nullable: false),
                        VerzamelLocatieLat = c.Single(nullable: false),
                        VerzamelAdres = c.String(nullable: false),
                        VerzamelPostcode = c.String(nullable: false, maxLength: 6),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EventPlans",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AantalDeelnemers = c.Int(nullable: false),
                        AantalGangen = c.Int(nullable: false),
                        AantalGroepen = c.Int(nullable: false),
                        AantalDuosPerGroep = c.Int(nullable: false),
                        Naam = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PostcodeGeoLocationCaches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Postcode = c.String(maxLength: 6),
                        NummerMin = c.Int(nullable: false),
                        NummerMax = c.Int(nullable: false),
                        NummerType = c.String(maxLength: 5),
                        GeoLong = c.Single(nullable: false),
                        GeoLat = c.Single(nullable: false),
                        rd_x = c.Single(nullable: false),
                        rd_y = c.Single(nullable: false),
                        Stad = c.String(nullable: false, maxLength: 32),
                        Straat = c.String(nullable: false, maxLength: 48),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Postcode);
            
            CreateTable(
                "dbo.DuoGroeps",
                c => new
                    {
                        Duo_Id = c.Int(nullable: false),
                        Groep_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Duo_Id, t.Groep_Id })
                .ForeignKey("dbo.Duos", t => t.Duo_Id, cascadeDelete: true)
                .ForeignKey("dbo.Groeps", t => t.Groep_Id, cascadeDelete: true)
                .Index(t => t.Duo_Id)
                .Index(t => t.Groep_Id);
            
            CreateTable(
                "dbo.EventPlanDuos",
                c => new
                    {
                        EventPlan_Id = c.Int(nullable: false),
                        Duo_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.EventPlan_Id, t.Duo_Id })
                .ForeignKey("dbo.EventPlans", t => t.EventPlan_Id, cascadeDelete: true)
                .ForeignKey("dbo.Duos", t => t.Duo_Id, cascadeDelete: true)
                .Index(t => t.EventPlan_Id)
                .Index(t => t.Duo_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EventPlanDuos", "Duo_Id", "dbo.Duos");
            DropForeignKey("dbo.EventPlanDuos", "EventPlan_Id", "dbo.EventPlans");
            DropForeignKey("dbo.DuoGroeps", "Groep_Id", "dbo.Groeps");
            DropForeignKey("dbo.DuoGroeps", "Duo_Id", "dbo.Duos");
            DropForeignKey("dbo.Groeps", "Host_Id", "dbo.Duos");
            DropForeignKey("dbo.Groeps", "Gang_Id", "dbo.Gangs");
            DropForeignKey("dbo.Gangs", "Schema_Id", "dbo.EventSchemas");
            DropIndex("dbo.EventPlanDuos", new[] { "Duo_Id" });
            DropIndex("dbo.EventPlanDuos", new[] { "EventPlan_Id" });
            DropIndex("dbo.DuoGroeps", new[] { "Groep_Id" });
            DropIndex("dbo.DuoGroeps", new[] { "Duo_Id" });
            DropIndex("dbo.PostcodeGeoLocationCaches", new[] { "Postcode" });
            DropIndex("dbo.Gangs", new[] { "Schema_Id" });
            DropIndex("dbo.Groeps", new[] { "Host_Id" });
            DropIndex("dbo.Groeps", new[] { "Gang_Id" });
            DropTable("dbo.EventPlanDuos");
            DropTable("dbo.DuoGroeps");
            DropTable("dbo.PostcodeGeoLocationCaches");
            DropTable("dbo.EventPlans");
            DropTable("dbo.EventSchemas");
            DropTable("dbo.Gangs");
            DropTable("dbo.Groeps");
            DropTable("dbo.Duos");
        }
    }
}
