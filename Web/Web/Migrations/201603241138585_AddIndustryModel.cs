namespace Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIndustryModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IndustryLists",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        IndustryName = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Companies", "Industry", c => c.String());
            AddColumn("dbo.Companies", "Plan_Id", c => c.Guid());
            CreateIndex("dbo.Companies", "Plan_Id");
            AddForeignKey("dbo.Companies", "Plan_Id", "dbo.Materials", "Id");
            DropColumn("dbo.Companies", "Name");
            DropColumn("dbo.Companies", "SourcesOfFunds");
            DropColumn("dbo.Companies", "Corporation");
            DropColumn("dbo.Companies", "RegisterSite");
            DropColumn("dbo.Companies", "CodeOfBusinessLicense");
            DropColumn("dbo.Companies", "MembersCount");
            DropColumn("dbo.Companies", "AmountOfMoney");
            DropColumn("dbo.Companies", "RecentTurnover");
            DropColumn("dbo.Companies", "CompanyValuation");
            DropColumn("dbo.Companies", "FinancingAmount");
            DropColumn("dbo.Companies", "SharesSold");
            DropColumn("dbo.Companies", "FinancingTime");
            DropColumn("dbo.Companies", "Investment");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Companies", "Investment", c => c.String());
            AddColumn("dbo.Companies", "FinancingTime", c => c.String());
            AddColumn("dbo.Companies", "SharesSold", c => c.Double(nullable: false));
            AddColumn("dbo.Companies", "FinancingAmount", c => c.Int(nullable: false));
            AddColumn("dbo.Companies", "CompanyValuation", c => c.Int(nullable: false));
            AddColumn("dbo.Companies", "RecentTurnover", c => c.Int(nullable: false));
            AddColumn("dbo.Companies", "AmountOfMoney", c => c.Int(nullable: false));
            AddColumn("dbo.Companies", "MembersCount", c => c.Int(nullable: false));
            AddColumn("dbo.Companies", "CodeOfBusinessLicense", c => c.String());
            AddColumn("dbo.Companies", "RegisterSite", c => c.String());
            AddColumn("dbo.Companies", "Corporation", c => c.String());
            AddColumn("dbo.Companies", "SourcesOfFunds", c => c.String());
            AddColumn("dbo.Companies", "Name", c => c.String());
            DropForeignKey("dbo.Companies", "Plan_Id", "dbo.Materials");
            DropIndex("dbo.Companies", new[] { "Plan_Id" });
            DropColumn("dbo.Companies", "Plan_Id");
            DropColumn("dbo.Companies", "Industry");
            DropTable("dbo.IndustryLists");
        }
    }
}
