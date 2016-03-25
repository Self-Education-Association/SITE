namespace Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeTimeColumnType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ActivityRecords", "ActionTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.ActivityRecords", "Time", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.CourseRecords", "ActionTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.CourseRecords", "Time", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.RoomRecords", "ActionTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.RoomRecords", "Time", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TeamRecords", "ActionTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TeamRecords", "Time", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TeamRecords", "Time", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TeamRecords", "ActionTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.RoomRecords", "Time", c => c.DateTime(nullable: false));
            AlterColumn("dbo.RoomRecords", "ActionTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.CourseRecords", "Time", c => c.DateTime(nullable: false));
            AlterColumn("dbo.CourseRecords", "ActionTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ActivityRecords", "Time", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ActivityRecords", "ActionTime", c => c.DateTime(nullable: false));
        }
    }
}
