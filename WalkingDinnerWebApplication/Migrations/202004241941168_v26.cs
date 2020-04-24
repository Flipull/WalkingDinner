namespace WalkingDinnerWebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v26 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EventSchemas", "VerzamelLocatieLong", c => c.Single(nullable: false));
            AlterColumn("dbo.EventSchemas", "VerzamelLocatieLat", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EventSchemas", "VerzamelLocatieLat", c => c.Double(nullable: false));
            AlterColumn("dbo.EventSchemas", "VerzamelLocatieLong", c => c.Double(nullable: false));
        }
    }
}
