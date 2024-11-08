﻿using Microsoft.EntityFrameworkCore;
using RecipeCubeWebService.Models;
namespace RecipeWebService.Models;

public partial class RecipeCubeContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration =
                new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("RecipeCube"));
        }
    }

public DbSet<RecipeCubeWebService.Models.Recipe> Recipe { get; set; } = default!;

    public static implicit operator RecipeCubeContext(RecipeCubeWebService.Models.RecipeCubeContext v)
    {
        throw new NotImplementedException();
    }
}
