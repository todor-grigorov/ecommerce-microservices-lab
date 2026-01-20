using ECommerce.Services.ProductAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services.ProdictAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 1,
                Name = "Shopska Salad",
                Price = 11,
                Description = "A refreshing mix of chopped tomatoes, cucumbers, onions, peppers, and grated sirene (white brine cheese), named after the Shops region.",
                ImageUrl = "https://placehold.co/603x403",
                CategoryName = "Appetizer"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 2,
                Name = "Snezhanka salad",
                Price = 9.99,
                Description = "Traditional Bulgarian salad, which is made of strained yogurt, cucumber, garlic, salt, usually cooking oil, dill, sometimes roasted peppers, walnuts and parsley.",
                ImageUrl = "https://placehold.co/602x402",
                CategoryName = "Appetizer"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 3,
                Name = "Tikvenik",
                Price = 10.99,
                Description = "Pastry consisting of thin sheets of dough that are filled with grated pumpkin, coarsely ground walnuts, sugar, and cinnamon.",
                ImageUrl = "https://placehold.co/601x401",
                CategoryName = "Dessert"
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 4,
                Name = "Kavarma ",
                Price = 15,
                Description = "Kavarma is a hearty, slow-cooked stew from Bulgarian cuisine, typically made with chunks of pork, chicken, or beef, simmered with vegetables like onions, peppers, tomatoes, and mushrooms, often in a traditional clay pot (gyuveche) with spices like paprika and savory (chubritsa), resulting in a rich, flavorful main course, sometimes finished with an egg.",
                ImageUrl = "https://placehold.co/600x400",
                CategoryName = "Main"
            });
        }
    }
}
