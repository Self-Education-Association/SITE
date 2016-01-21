namespace Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActivityOperations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Count = c.Int(nullable: false),
                        Limit = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Time = c.DateTime(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        Content = c.String(),
                        ShortContent = c.String(maxLength: 50),
                        Enabled = c.Boolean(nullable: false),
                        Creator_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id)
                .Index(t => t.Creator_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        DisplayName = c.String(),
                        Profile_Email = c.String(),
                        Profile_Phone = c.String(),
                        Profile_Other = c.String(),
                        Profile_InformationPrivacy = c.Boolean(nullable: false),
                        Profile_Searchable = c.Boolean(nullable: false),
                        Identitied = c.Boolean(nullable: false),
                        Time = c.DateTime(nullable: false),
                        IsDisabled = c.Boolean(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        CourseOperation_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CourseOperations", t => t.CourseOperation_Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex")
                .Index(t => t.CourseOperation_Id);
            
            CreateTable(
                "dbo.ActivityRecords",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ActionTime = c.DateTime(nullable: false),
                        RemarkContent = c.String(),
                        RemarkRate = c.Int(nullable: false),
                        Time = c.DateTime(nullable: false),
                        ActivityOperation_Id = c.Guid(),
                        Receiver_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ActivityOperations", t => t.ActivityOperation_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Receiver_Id)
                .Index(t => t.ActivityOperation_Id)
                .Index(t => t.Receiver_Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.CourseRecords",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ActionTime = c.DateTime(nullable: false),
                        RemarkContent = c.String(),
                        RemarkRate = c.Int(nullable: false),
                        Time = c.DateTime(nullable: false),
                        CourseOperation_Id = c.Guid(),
                        Receiver_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CourseOperations", t => t.CourseOperation_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Receiver_Id)
                .Index(t => t.CourseOperation_Id)
                .Index(t => t.Receiver_Id);
            
            CreateTable(
                "dbo.CourseOperations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Count = c.Int(nullable: false),
                        Limit = c.Int(nullable: false),
                        Status = c.String(),
                        Location = c.String(),
                        Name = c.String(nullable: false),
                        Time = c.DateTime(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        Content = c.String(),
                        ShortContent = c.String(maxLength: 50),
                        Enabled = c.Boolean(nullable: false),
                        Creator_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id)
                .Index(t => t.Creator_Id);
            
            CreateTable(
                "dbo.EducationRecords",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        StartYear = c.DateTime(nullable: false, storeType: "date"),
                        EndYear = c.DateTime(nullable: false, storeType: "date"),
                        School = c.String(nullable: false),
                        DegreeType = c.Int(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.IdentityRecords",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        IdNumber = c.String(),
                        Name = c.String(),
                        InUIBE = c.Boolean(nullable: false),
                        IsStudent = c.Boolean(nullable: false),
                        Time = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        BackIdCard_Id = c.Guid(),
                        FrontIdCard_Id = c.Guid(),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Materials", t => t.BackIdCard_Id)
                .ForeignKey("dbo.Materials", t => t.FrontIdCard_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.BackIdCard_Id)
                .Index(t => t.FrontIdCard_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Materials",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        Time = c.DateTime(nullable: false),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Industry = c.String(nullable: false),
                        Introduction = c.String(nullable: false),
                        Product = c.String(nullable: false),
                        Feature = c.String(nullable: false),
                        Tech = c.String(nullable: false),
                        Progress = c.Int(nullable: false),
                        Patent = c.String(nullable: false),
                        Desire = c.String(nullable: false),
                        TargetCustomer = c.String(nullable: false),
                        ProjectBudget = c.String(nullable: false),
                        Webpage = c.String(),
                        Privacy = c.Boolean(nullable: false),
                        Note = c.String(),
                        Time = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        Avatar_Id = c.Guid(),
                        Team_Id = c.Guid(),
                        Admin_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Materials", t => t.Avatar_Id)
                .ForeignKey("dbo.Teams", t => t.Team_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Admin_Id)
                .Index(t => t.Avatar_Id)
                .Index(t => t.Team_Id)
                .Index(t => t.Admin_Id);
            
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Time = c.DateTime(nullable: false),
                        Introduction = c.String(),
                        Announcement = c.String(),
                        Searchable = c.Boolean(nullable: false),
                        Admin_Id = c.String(maxLength: 128),
                        Company_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Admin_Id)
                .ForeignKey("dbo.Companies", t => t.Company_Id)
                .Index(t => t.Admin_Id)
                .Index(t => t.Company_Id);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        SourcesOfFunds = c.String(),
                        Corporation = c.String(),
                        RegisterSite = c.String(),
                        CodeOfBusinessLicense = c.String(),
                        MembersCount = c.Int(nullable: false),
                        AmountOfMoney = c.Int(nullable: false),
                        RecentTurnover = c.Int(nullable: false),
                        CompanyValuation = c.Int(nullable: false),
                        FinancingAmount = c.Int(nullable: false),
                        SharesSold = c.Double(nullable: false),
                        FinancingTime = c.String(),
                        Investment = c.String(),
                        Note = c.String(),
                        Time = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        Admin_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Admin_Id)
                .Index(t => t.Admin_Id);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.RoomRecords",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ActionTime = c.DateTime(nullable: false),
                        RemarkContent = c.String(),
                        RemarkRate = c.Int(nullable: false),
                        Time = c.DateTime(nullable: false),
                        Receiver_Id = c.String(maxLength: 128),
                        RoomOperation_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Receiver_Id)
                .ForeignKey("dbo.RoomOperations", t => t.RoomOperation_Id)
                .Index(t => t.Receiver_Id)
                .Index(t => t.RoomOperation_Id);
            
            CreateTable(
                "dbo.RoomOperations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Usable = c.Boolean(nullable: false),
                        Name = c.String(nullable: false),
                        Time = c.DateTime(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        Content = c.String(),
                        ShortContent = c.String(maxLength: 50),
                        Enabled = c.Boolean(nullable: false),
                        Creator_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id)
                .Index(t => t.Creator_Id);
            
            CreateTable(
                "dbo.TeamRecords",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        ActionTime = c.DateTime(nullable: false),
                        RemarkContent = c.String(),
                        RemarkRate = c.Int(nullable: false),
                        Time = c.DateTime(nullable: false),
                        Receiver_Id = c.String(nullable: false, maxLength: 128),
                        Team_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Receiver_Id)
                .ForeignKey("dbo.Teams", t => t.Team_Id)
                .Index(t => t.Receiver_Id)
                .Index(t => t.Team_Id);
            
            CreateTable(
                "dbo.WorkRecords",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        StartYear = c.DateTime(nullable: false, storeType: "date"),
                        EndYear = c.DateTime(nullable: false, storeType: "date"),
                        Company = c.String(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Articles",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        Title = c.String(),
                        ContentStored = c.String(),
                        ShortContent = c.String(),
                        Status = c.Int(nullable: false),
                        Class = c.Int(nullable: false),
                        Time = c.DateTime(nullable: false),
                        Image_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Materials", t => t.Image_Id)
                .Index(t => t.Image_Id);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        Title = c.String(),
                        Content = c.String(),
                        Type = c.Int(nullable: false),
                        HaveRead = c.Boolean(nullable: false),
                        Time = c.DateTime(nullable: false),
                        Publisher_Id = c.String(maxLength: 128),
                        Receiver_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.Publisher_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Receiver_Id)
                .Index(t => t.Publisher_Id)
                .Index(t => t.Receiver_Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
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
            
            CreateTable(
                "dbo.TutorInformations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Introduction = c.String(),
                        Position = c.String(),
                        Avatar_Id = c.Guid(),
                        Tutor_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Materials", t => t.Avatar_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Tutor_Id)
                .Index(t => t.Avatar_Id)
                .Index(t => t.Tutor_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TutorInformations", "Tutor_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.TutorInformations", "Avatar_Id", "dbo.Materials");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Messages", "Receiver_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Messages", "Publisher_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Articles", "Image_Id", "dbo.Materials");
            DropForeignKey("dbo.ActivityOperations", "Creator_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.WorkRecords", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.TeamRecords", "Team_Id", "dbo.Teams");
            DropForeignKey("dbo.TeamRecords", "Receiver_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.RoomRecords", "RoomOperation_Id", "dbo.RoomOperations");
            DropForeignKey("dbo.RoomOperations", "Creator_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.RoomRecords", "Receiver_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Projects", "Admin_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Projects", "Team_Id", "dbo.Teams");
            DropForeignKey("dbo.Teams", "Company_Id", "dbo.Companies");
            DropForeignKey("dbo.Companies", "Admin_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Teams", "Admin_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Projects", "Avatar_Id", "dbo.Materials");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.IdentityRecords", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.IdentityRecords", "FrontIdCard_Id", "dbo.Materials");
            DropForeignKey("dbo.IdentityRecords", "BackIdCard_Id", "dbo.Materials");
            DropForeignKey("dbo.EducationRecords", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.CourseRecords", "Receiver_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.CourseRecords", "CourseOperation_Id", "dbo.CourseOperations");
            DropForeignKey("dbo.AspNetUsers", "CourseOperation_Id", "dbo.CourseOperations");
            DropForeignKey("dbo.CourseOperations", "Creator_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ActivityRecords", "Receiver_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ActivityRecords", "ActivityOperation_Id", "dbo.ActivityOperations");
            DropIndex("dbo.TutorInformations", new[] { "Tutor_Id" });
            DropIndex("dbo.TutorInformations", new[] { "Avatar_Id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Messages", new[] { "Receiver_Id" });
            DropIndex("dbo.Messages", new[] { "Publisher_Id" });
            DropIndex("dbo.Articles", new[] { "Image_Id" });
            DropIndex("dbo.WorkRecords", new[] { "User_Id" });
            DropIndex("dbo.TeamRecords", new[] { "Team_Id" });
            DropIndex("dbo.TeamRecords", new[] { "Receiver_Id" });
            DropIndex("dbo.RoomOperations", new[] { "Creator_Id" });
            DropIndex("dbo.RoomRecords", new[] { "RoomOperation_Id" });
            DropIndex("dbo.RoomRecords", new[] { "Receiver_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.Companies", new[] { "Admin_Id" });
            DropIndex("dbo.Teams", new[] { "Company_Id" });
            DropIndex("dbo.Teams", new[] { "Admin_Id" });
            DropIndex("dbo.Projects", new[] { "Admin_Id" });
            DropIndex("dbo.Projects", new[] { "Team_Id" });
            DropIndex("dbo.Projects", new[] { "Avatar_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.IdentityRecords", new[] { "User_Id" });
            DropIndex("dbo.IdentityRecords", new[] { "FrontIdCard_Id" });
            DropIndex("dbo.IdentityRecords", new[] { "BackIdCard_Id" });
            DropIndex("dbo.EducationRecords", new[] { "User_Id" });
            DropIndex("dbo.CourseOperations", new[] { "Creator_Id" });
            DropIndex("dbo.CourseRecords", new[] { "Receiver_Id" });
            DropIndex("dbo.CourseRecords", new[] { "CourseOperation_Id" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.ActivityRecords", new[] { "Receiver_Id" });
            DropIndex("dbo.ActivityRecords", new[] { "ActivityOperation_Id" });
            DropIndex("dbo.AspNetUsers", new[] { "CourseOperation_Id" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.ActivityOperations", new[] { "Creator_Id" });
            DropTable("dbo.TutorInformations");
            DropTable("dbo.TeamProfileViewModels");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Messages");
            DropTable("dbo.Articles");
            DropTable("dbo.WorkRecords");
            DropTable("dbo.TeamRecords");
            DropTable("dbo.RoomOperations");
            DropTable("dbo.RoomRecords");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.Companies");
            DropTable("dbo.Teams");
            DropTable("dbo.Projects");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.Materials");
            DropTable("dbo.IdentityRecords");
            DropTable("dbo.EducationRecords");
            DropTable("dbo.CourseOperations");
            DropTable("dbo.CourseRecords");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.ActivityRecords");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.ActivityOperations");
        }
    }
}
