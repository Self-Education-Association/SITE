namespace Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class UpdateUserModelIdentityModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SchoolLists",
                c => new
                {
                    Id = c.Guid(nullable: false, defaultValue: Guid.NewGuid()),
                    Name = c.String(),
                })
                .PrimaryKey(t => t.Id);

            AddColumn("dbo.AspNetUsers", "School", c => c.String());
            DropColumn("dbo.IdentityRecords", "InUIBE");
            DropColumn("dbo.IdentityRecords", "IsStudent");
        }

        public override void Down()
        {
            AddColumn("dbo.IdentityRecords", "IsStudent", c => c.Boolean(nullable: false));
            AddColumn("dbo.IdentityRecords", "InUIBE", c => c.Boolean(nullable: false));
            DropColumn("dbo.AspNetUsers", "School");
            DropTable("dbo.SchoolLists");
        }
    }
}
