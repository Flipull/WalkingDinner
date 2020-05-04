namespace WalkingDinnerWebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Duos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PostCode = c.String(nullable: false, maxLength: 6),
                        Huisnummer = c.String(nullable: false, maxLength: 8),
                        GeoLong = c.Double(nullable: false),
                        GeoLat = c.Double(nullable: false),
                        Naam = c.String(nullable: false),
                        Groep_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Groeps", t => t.Groep_Id)
                .Index(t => t.Groep_Id);
            
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
                "dbo.EventSchemas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VerzamelDatum = c.DateTime(nullable: false),
                        VerzamelLocatieLong = c.Double(nullable: false),
                        VerzamelLocatieLat = c.Double(nullable: false),
                        VerzamelAdres = c.String(nullable: false),
                        VerzamelPostcode = c.String(nullable: false, maxLength: 6),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Gangs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartTijd = c.DateTime(nullable: false),
                        EindTijd = c.DateTime(nullable: false),
                        EventSchema_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EventSchemas", t => t.EventSchema_Id)
                .Index(t => t.EventSchema_Id);
            
            CreateTable(
                "dbo.Groeps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Host_Id = c.Int(),
                        Gang_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Duos", t => t.Host_Id)
                .ForeignKey("dbo.Gangs", t => t.Gang_Id)
                .Index(t => t.Host_Id)
                .Index(t => t.Gang_Id);
            
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
            DropForeignKey("dbo.Gangs", "EventSchema_Id", "dbo.EventSchemas");
            DropForeignKey("dbo.Groeps", "Gang_Id", "dbo.Gangs");
            DropForeignKey("dbo.Groeps", "Host_Id", "dbo.Duos");
            DropForeignKey("dbo.Duos", "Groep_Id", "dbo.Groeps");
            DropForeignKey("dbo.EventPlanDuos", "Duo_Id", "dbo.Duos");
            DropForeignKey("dbo.EventPlanDuos", "EventPlan_Id", "dbo.EventPlans");
            DropIndex("dbo.EventPlanDuos", new[] { "Duo_Id" });
            DropIndex("dbo.EventPlanDuos", new[] { "EventPlan_Id" });
            DropIndex("dbo.Groeps", new[] { "Gang_Id" });
            DropIndex("dbo.Groeps", new[] { "Host_Id" });
            DropIndex("dbo.Gangs", new[] { "EventSchema_Id" });
            DropIndex("dbo.Duos", new[] { "Groep_Id" });
            DropTable("dbo.EventPlanDuos");
            DropTable("dbo.Groeps");
            DropTable("dbo.Gangs");
            DropTable("dbo.EventSchemas");
            DropTable("dbo.EventPlans");
            DropTable("dbo.Duos");
        }
    }
}
