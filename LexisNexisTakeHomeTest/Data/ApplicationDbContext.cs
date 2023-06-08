using LexisNexisTakeHomeTest.Models;
using Microsoft.EntityFrameworkCore;

namespace LexisNexisTakeHomeTest.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Instruction> Instructions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ingredient>()
                .HasIndex(i => i.RecipeId)
                .HasDatabaseName("IX_Ingredients_RecipeId");

            modelBuilder.Entity<Instruction>()
                .HasIndex(i => i.RecipeId)
                .HasDatabaseName("IX_Instructions_RecipeId");
        }

    }
}
