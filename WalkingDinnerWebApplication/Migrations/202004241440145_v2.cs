namespace WalkingDinnerWebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PostcodeGeoLocationCaches", "NummerStart", c => c.Int(nullable: false));
            AddColumn("dbo.PostcodeGeoLocationCaches", "NummerEind", c => c.Int(nullable: false));
            AddColumn("dbo.PostcodeGeoLocationCaches", "NummerType", c => c.String(nullable: false, maxLength: 5));
            AlterColumn("dbo.PostcodeGeoLocationCaches", "Stad", c => c.String(nullable: false, maxLength: 32));
            AlterColumn("dbo.PostcodeGeoLocationCaches", "Straat", c => c.String(nullable: false, maxLength: 48));
            AlterColumn("dbo.PostcodeGeoLocationCaches", "GeoLong", c => c.Double(nullable: false));
            AlterColumn("dbo.PostcodeGeoLocationCaches", "GeoLat", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PostcodeGeoLocationCaches", "GeoLat", c => c.Double());
            AlterColumn("dbo.PostcodeGeoLocationCaches", "GeoLong", c => c.Double());
            AlterColumn("dbo.PostcodeGeoLocationCaches", "Straat", c => c.String(maxLength: 64));
            AlterColumn("dbo.PostcodeGeoLocationCaches", "Stad", c => c.String(maxLength: 64));
            DropColumn("dbo.PostcodeGeoLocationCaches", "NummerType");
            DropColumn("dbo.PostcodeGeoLocationCaches", "NummerEind");
            DropColumn("dbo.PostcodeGeoLocationCaches", "NummerStart");
        }
    }
}
