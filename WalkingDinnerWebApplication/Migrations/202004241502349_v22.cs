namespace WalkingDinnerWebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v22 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PostcodeGeoLocationCaches", "NummerType", c => c.String(maxLength: 5));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PostcodeGeoLocationCaches", "NummerType", c => c.String(nullable: false, maxLength: 5));
        }
    }
}
