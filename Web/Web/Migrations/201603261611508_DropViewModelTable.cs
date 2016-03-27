namespace Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropViewModelTable : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.TeamProfileViewModels");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TeamProfileViewModels",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Administrator = c.String(),
                        Time = c.DateTime(nullable: false),
                        Introduction = c.String(maxLength: 100),
                        Announcement = c.String(maxLength: 100),
                        Searchable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
