namespace Web.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Web.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<Web.Models.BaseDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Web.Models.BaseDbContext";
        }

        protected override void Seed(Web.Models.BaseDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            context.Materials.AddOrUpdate(m => m.Id, new Material { Id = Guid.Empty.DefaultMaterial(DefaultMaterial.News), Name = "DefaultNews.jpg", Time = DateTime.Now, Type = MaterialType.Avatar }, new Material { Id = Guid.Empty.DefaultMaterial(DefaultMaterial.Avatar), Name = "DefaultAvatar.png", Time = DateTime.Now, Type = MaterialType.Avatar });
        }
    }
}
