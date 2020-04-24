namespace WalkingDinnerWebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v24 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Duos", "Huisnummer", c => c.Int(nullable: false));
            AlterColumn("dbo.Duos", "GeoLong", c => c.Single(nullable: false));
            AlterColumn("dbo.Duos", "GeoLat", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Duos", "GeoLat", c => c.Double(nullable: false));
            AlterColumn("dbo.Duos", "GeoLong", c => c.Double(nullable: false));
            AlterColumn("dbo.Duos", "Huisnummer", c => c.String(nullable: false, maxLength: 8));
        }
    }
}
