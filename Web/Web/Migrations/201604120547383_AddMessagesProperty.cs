namespace Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMessagesProperty : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Messages", "Receiver_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Messages", new[] { "Receiver_Id" });
            AlterColumn("dbo.Messages", "Receiver_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Messages", "Receiver_Id");
            AddForeignKey("dbo.Messages", "Receiver_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "Receiver_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Messages", new[] { "Receiver_Id" });
            AlterColumn("dbo.Messages", "Receiver_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Messages", "Receiver_Id");
            AddForeignKey("dbo.Messages", "Receiver_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
