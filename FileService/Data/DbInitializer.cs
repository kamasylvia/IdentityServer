using System;
using System.IO;
using System.Linq;
using FileService.Data;
using FileService.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FileService.Data
{
    public class DbInitializer
    {
        public static void Initialize(IApplicationBuilder app, IWebHostEnvironment environment)
        {
            using (
                var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()
                    .CreateScope()
            ) {
                var context =
                    serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (environment.IsDevelopment())
                {
                    var deleted = context.Database.EnsureDeleted();
                    System.Console.WriteLine($"The old database is deleted: {deleted}");
                    var created = context.Database.EnsureCreated();
                    System.Console.WriteLine($"The new database is created: {created}");
                }

                if (environment.IsProduction())
                {
                    context.Database.Migrate();
                }

                // Look for any Data.
                if (context.Files.Any())
                {
                    return; // DB has been seeded
                }

                SeedData(context);

                var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
                Directory.CreateDirectory(configuration["FileStorageDirectory"]);
            }
        }

        private static void SeedData(ApplicationDbContext context)
        {
            var seedPhoto = new AppFile
            {
                OwnerId = Guid.NewGuid(),
                Key = "Seed",
                ContentType = "application/json"
            };
            context.Files.Add(seedPhoto);
            context.SaveChanges();
        }
    }
}
