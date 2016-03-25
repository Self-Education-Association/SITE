namespace Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCourseModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ActivityOperations", "ContentStored", c => c.String());
            AddColumn("dbo.ActivityOperations", "ShortContentStored", c => c.String(maxLength: 50));
            AddColumn("dbo.CourseOperations", "ContentStored", c => c.String());
            AddColumn("dbo.CourseOperations", "ShortContentStored", c => c.String(maxLength: 50));
            AddColumn("dbo.RoomOperations", "ContentStored", c => c.String());
            AddColumn("dbo.RoomOperations", "ShortContentStored", c => c.String(maxLength: 50));
            AddColumn("dbo.Articles", "ShortContentStored", c => c.String());
            AlterColumn("dbo.CourseOperations", "Status", c => c.Boolean(nullable: false));
            DropColumn("dbo.ActivityOperations", "Content");
            DropColumn("dbo.ActivityOperations", "ShortContent");
            DropColumn("dbo.CourseOperations", "Content");
            DropColumn("dbo.CourseOperations", "ShortContent");
            DropColumn("dbo.RoomOperations", "Content");
            DropColumn("dbo.RoomOperations", "ShortContent");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RoomOperations", "ShortContent", c => c.String(maxLength: 50));
            AddColumn("dbo.RoomOperations", "Content", c => c.String());
            AddColumn("dbo.CourseOperations", "ShortContent", c => c.String(maxLength: 50));
            AddColumn("dbo.CourseOperations", "Content", c => c.String());
            AddColumn("dbo.ActivityOperations", "ShortContent", c => c.String(maxLength: 50));
            AddColumn("dbo.ActivityOperations", "Content", c => c.String());
            AlterColumn("dbo.CourseOperations", "Status", c => c.String());
            DropColumn("dbo.Articles", "ShortContentStored");
            DropColumn("dbo.RoomOperations", "ShortContentStored");
            DropColumn("dbo.RoomOperations", "ContentStored");
            DropColumn("dbo.CourseOperations", "ShortContentStored");
            DropColumn("dbo.CourseOperations", "ContentStored");
            DropColumn("dbo.ActivityOperations", "ShortContentStored");
            DropColumn("dbo.ActivityOperations", "ContentStored");
        }
    }
}
