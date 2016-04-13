namespace Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTeamModels : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Teams", "ReportUpdated", c => c.Boolean(nullable: false));
            AddColumn("dbo.TeamReports", "Round", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TeamReports", "Round");
            DropColumn("dbo.Teams", "ReportUpdated");
        }
    }
}
