using ECommerce.Services.OrderAPI.Dto;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Services.OrderAPI.Models
{
    public class OrderDetails
    {
        public int OrderDetailsId { get; set; }
        public int OrderHeaderId { get; set; }

        [ForeignKey("OrderHeaderId")]
        public OrderHeader? OrderHeader { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }

        [NotMapped]
        public ProductDto? Product { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
    }
}
