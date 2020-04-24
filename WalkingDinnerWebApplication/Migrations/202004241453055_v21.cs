namespace WalkingDinnerWebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v21 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PostcodeGeoLocationCaches", "NummerMin", c => c.Int(nullable: false));
            AddColumn("dbo.PostcodeGeoLocationCaches", "NummerMax", c => c.Int(nullable: false));
            AddColumn("dbo.PostcodeGeoLocationCaches", "rd_x", c => c.Single(nullable: false));
            AddColumn("dbo.PostcodeGeoLocationCaches", "rd_y", c => c.Single(nullable: false));
            AlterColumn("dbo.PostcodeGeoLocationCaches", "GeoLong", c => c.Single(nullable: false));
            AlterColumn("dbo.PostcodeGeoLocationCaches", "GeoLat", c => c.Single(nullable: false));
            DropColumn("dbo.PostcodeGeoLocationCaches", "NummerStart");
            DropColumn("dbo.PostcodeGeoLocationCaches", "NummerEind");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PostcodeGeoLocationCaches", "NummerEind", c => c.Int(nullable: false));
            AddColumn("dbo.PostcodeGeoLocationCaches", "NummerStart", c => c.Int(nullable: false));
            AlterColumn("dbo.PostcodeGeoLocationCaches", "GeoLat", c => c.Double(nullable: false));
            AlterColumn("dbo.PostcodeGeoLocationCaches", "GeoLong", c => c.Double(nullable: false));
            DropColumn("dbo.PostcodeGeoLocationCaches", "rd_y");
            DropColumn("dbo.PostcodeGeoLocationCaches", "rd_x");
            DropColumn("dbo.PostcodeGeoLocationCaches", "NummerMax");
            DropColumn("dbo.PostcodeGeoLocationCaches", "NummerMin");
        }
    }
}
