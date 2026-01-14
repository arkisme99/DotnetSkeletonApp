using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSkeletonApp.Data;
using DotnetSkeletonApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetSkeletonApp.Seeders
{
    public static class SetupAppSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {

            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            var nameApp = "Skeleton Dotnet App";

            if (!await dbContext.Setups.AnyAsync())
            {
                var newSetup = new Setup
                {
                    // Id = Guid.NewGuid(), // Karena Anda menggunakan Guid
                    NameApp = nameApp
                };

                await dbContext.Setups.AddAsync(newSetup);
                await dbContext.SaveChangesAsync();
            }

        }
    }
}