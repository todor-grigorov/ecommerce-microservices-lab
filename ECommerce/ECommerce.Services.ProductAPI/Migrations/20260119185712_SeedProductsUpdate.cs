using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Services.ProductAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedProductsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                columns: new[] { "Description", "ImageUrl", "Name", "Price" },
                values: new object[] { "A refreshing mix of chopped tomatoes, cucumbers, onions, peppers, and grated sirene (white brine cheese), named after the Shops region.", "https://placehold.co/600x400", "Shopska Salad", 11.0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                columns: new[] { "Description", "ImageUrl", "Name", "Price" },
                values: new object[] { "Traditional Bulgarian salad, which is made of strained yogurt, cucumber, garlic, salt, usually cooking oil, dill, sometimes roasted peppers, walnuts and parsley.", "https://placehold.co/600x400", "Snezhanka salad", 9.9900000000000002 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Pastry consisting of thin sheets of dough that are filled with grated pumpkin, coarsely ground walnuts, sugar, and cinnamon.", "https://placehold.co/600x400", "Tikvenik " });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4,
                columns: new[] { "CategoryName", "Description", "Name" },
                values: new object[] { "Main", "Kavarma is a hearty, slow-cooked stew from Bulgarian cuisine, typically made with chunks of pork, chicken, or beef, simmered with vegetables like onions, peppers, tomatoes, and mushrooms, often in a traditional clay pot (gyuveche) with spices like paprika and savory (chubritsa), resulting in a rich, flavorful main course, sometimes finished with an egg. ", "Kavarma " });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                columns: new[] { "Description", "ImageUrl", "Name", "Price" },
                values: new object[] { " Quisque vel lacus ac magna, vehicula sagittis ut non lacus.<br/> Vestibulum arcu turpis, maximus malesuada neque. Phasellus commodo cursus pretium.", "https://placehold.co/603x403", "Samosa", 15.0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                columns: new[] { "Description", "ImageUrl", "Name", "Price" },
                values: new object[] { " Quisque vel lacus ac magna, vehicula sagittis ut non lacus.<br/> Vestibulum arcu turpis, maximus malesuada neque. Phasellus commodo cursus pretium.", "https://placehold.co/602x402", "Paneer Tikka", 13.99 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { " Quisque vel lacus ac magna, vehicula sagittis ut non lacus.<br/> Vestibulum arcu turpis, maximus malesuada neque. Phasellus commodo cursus pretium.", "https://placehold.co/601x401", "Sweet Pie" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4,
                columns: new[] { "CategoryName", "Description", "Name" },
                values: new object[] { "Entree", " Quisque vel lacus ac magna, vehicula sagittis ut non lacus.<br/> Vestibulum arcu turpis, maximus malesuada neque. Phasellus commodo cursus pretium.", "Pav Bhaji" });
        }
    }
}
