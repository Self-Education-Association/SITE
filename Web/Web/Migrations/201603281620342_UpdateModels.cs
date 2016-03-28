namespace Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateModels : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.IdentityRecords", "IdNumber", c => c.String(nullable: false));
            AlterColumn("dbo.IdentityRecords", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.IdentityRecords", "Name", c => c.String());
            AlterColumn("dbo.IdentityRecords", "IdNumber", c => c.String());
        }
    }
}
