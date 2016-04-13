namespace Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTeamEventModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TeamEvents", "EventTime", c => c.DateTime(nullable: false, precision: 0, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TeamEvents", "EventTime", c => c.DateTime(nullable: false, storeType: "date"));
        }
    }
}
