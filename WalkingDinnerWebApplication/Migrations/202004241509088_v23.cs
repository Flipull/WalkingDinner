namespace WalkingDinnerWebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v23 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.PostcodeGeoLocationCaches");
            AddColumn("dbo.PostcodeGeoLocationCaches", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.PostcodeGeoLocationCaches", "Postcode", c => c.String(maxLength: 6));
            AddPrimaryKey("dbo.PostcodeGeoLocationCaches", "Id");
            CreateIndex("dbo.PostcodeGeoLocationCaches", "Postcode");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PostcodeGeoLocationCaches", new[] { "Postcode" });
            DropPrimaryKey("dbo.PostcodeGeoLocationCaches");
            AlterColumn("dbo.PostcodeGeoLocationCaches", "Postcode", c => c.String(nullable: false, maxLength: 6));
            DropColumn("dbo.PostcodeGeoLocationCaches", "Id");
            AddPrimaryKey("dbo.PostcodeGeoLocationCaches", "Postcode");
        }
    }
}
