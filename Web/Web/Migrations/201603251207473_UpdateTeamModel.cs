namespace Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTeamModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TeamEvents",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EventName = c.String(),
                        EventContent = c.String(),
                        EventTime = c.DateTime(nullable: false, storeType: "date"),
                        AddTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Team_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Teams", t => t.Team_Id, cascadeDelete: true)
                .Index(t => t.Team_Id);
            
            CreateTable(
                "dbo.TeamReports",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Time = c.DateTime(nullable: false),
                        ReportFile_Id = c.Guid(nullable: false),
                        Team_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Materials", t => t.ReportFile_Id, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.Team_Id, cascadeDelete: true)
                .Index(t => t.ReportFile_Id)
                .Index(t => t.Team_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TeamReports", "Team_Id", "dbo.Teams");
            DropForeignKey("dbo.TeamReports", "ReportFile_Id", "dbo.Materials");
            DropForeignKey("dbo.TeamEvents", "Team_Id", "dbo.Teams");
            DropIndex("dbo.TeamReports", new[] { "Team_Id" });
            DropIndex("dbo.TeamReports", new[] { "ReportFile_Id" });
            DropIndex("dbo.TeamEvents", new[] { "Team_Id" });
            DropTable("dbo.TeamReports");
            DropTable("dbo.TeamEvents");
        }
    }
}
