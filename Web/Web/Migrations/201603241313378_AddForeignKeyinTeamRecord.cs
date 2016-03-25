namespace Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForeignKeyinTeamRecord : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TeamRecords", "Team_Id", "dbo.Teams");
            DropIndex("dbo.TeamRecords", new[] { "Team_Id" });
            AlterColumn("dbo.TeamRecords", "Team_Id", c => c.Guid(nullable: false));
            CreateIndex("dbo.TeamRecords", "Team_Id");
            AddForeignKey("dbo.TeamRecords", "Team_Id", "dbo.Teams", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TeamRecords", "Team_Id", "dbo.Teams");
            DropIndex("dbo.TeamRecords", new[] { "Team_Id" });
            AlterColumn("dbo.TeamRecords", "Team_Id", c => c.Guid());
            CreateIndex("dbo.TeamRecords", "Team_Id");
            AddForeignKey("dbo.TeamRecords", "Team_Id", "dbo.Teams", "Id");
        }
    }
}
