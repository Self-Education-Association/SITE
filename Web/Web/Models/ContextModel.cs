using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class BaseDbContext : IdentityDbContext<User>
    {
        public BaseDbContext()
            : base("DefaultConnection")
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<BaseDbContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasOptional(u => u.Project).WithRequired(p => p.Admin);
            modelBuilder.Entity<User>().HasOptional(u => u.IdentityRecord).WithRequired(i => i.User);
            modelBuilder.Entity<User>().HasOptional(u => u.Project).WithRequired(p => p.Admin);
            modelBuilder.Entity<TeamRecord>().HasRequired(t => t.Receiver).WithOptional(u => u.TeamRecord);
            modelBuilder.Entity<TeamRecord>().HasRequired(t => t.Team).WithMany(t => t.Member);
            modelBuilder.Entity<Team>().HasOptional(t => t.Company);
            modelBuilder.Entity<Team>().HasMany(t => t.Reports);
            modelBuilder.Entity<WorkRecord>().Property(w => w.StartYear).HasColumnType("Date");
            modelBuilder.Entity<WorkRecord>().Property(w => w.EndYear).HasColumnType("Date");
            modelBuilder.Entity<EducationRecord>().Property(w => w.StartYear).HasColumnType("Date");
            modelBuilder.Entity<EducationRecord>().Property(w => w.EndYear).HasColumnType("Date");
            modelBuilder.Entity<TeamRecord>().Property(t => t.Time).HasColumnType("Datetime2");
            modelBuilder.Entity<TeamRecord>().Property(t => t.ActionTime).HasColumnType("Datetime2");
            modelBuilder.Entity<RoomRecord>().Property(t => t.Time).HasColumnType("Datetime2");
            modelBuilder.Entity<RoomRecord>().Property(t => t.ActionTime).HasColumnType("Datetime2");
            modelBuilder.Entity<ActivityRecord>().Property(t => t.Time).HasColumnType("Datetime2");
            modelBuilder.Entity<ActivityRecord>().Property(t => t.ActionTime).HasColumnType("Datetime2");
            modelBuilder.Entity<CourseRecord>().Property(t => t.Time).HasColumnType("Datetime2");
            modelBuilder.Entity<CourseRecord>().Property(t => t.ActionTime).HasColumnType("Datetime2");
            modelBuilder.Entity<TeamEvent>().Property(t => t.AddTime).HasColumnType("Datetime2");
            modelBuilder.Entity<TeamEvent>().Property(t => t.EventTime).HasColumnType("Date");
            modelBuilder.Entity<TeamEvent>().HasRequired(t => t.Team).WithMany(t => t.Events);
            modelBuilder.Entity<TeamReport>().HasRequired(t => t.Team).WithMany(t => t.Reports);
            modelBuilder.Entity<TeamReport>().HasRequired(t => t.ReportFile);
        }

        public static BaseDbContext Create()
        {
            return new BaseDbContext();
        }

        public virtual DbSet<EducationRecord> EducationRecords { get; set; }

        public virtual DbSet<WorkRecord> WorkRecords { get; set; }

        public virtual DbSet<ActivityOperation> ActivityOperations { get; set; }

        public virtual DbSet<ActivityRecord> ActivityRecords { get; set; }

        public virtual DbSet<CourseOperation> CourseOperations { get; set; }

        public virtual DbSet<CourseRecord> CourseRecords { get; set; }

        public virtual DbSet<RoomOperation> RoomOperations { get; set; }

        public virtual DbSet<RoomRecord> RoomRecords { get; set; }

        public virtual DbSet<Material> Materials { get; set; }

        public virtual DbSet<Article> Articles { get; set; }

        public virtual DbSet<Message> Messages { get; set; }

        public virtual DbSet<Project> Projects { get; set; }

        public virtual DbSet<Team> Teams { get; set; }

        public virtual DbSet<TeamRecord> TeamRecords { get; set; }

        public virtual DbSet<Company> Companys { get; set; }

        public virtual DbSet<IdentityRecord> IdentityRecords { get; set; }

        public virtual DbSet<TutorInformation> TutorInformations { get; set; }

        public virtual DbSet<TeamEvent> TeamEvents { get; set; }

        public virtual DbSet<IndustryList> IndustryList { get; set; }

        public System.Data.Entity.DbSet<Web.Models.TeamReport> TeamReports { get; set; }
    }
}