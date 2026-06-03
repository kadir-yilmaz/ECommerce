using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.Discount
{
    public class CalculateDiscountRequest
    {
        public List<CalculateDiscountItem> Items { get; set; } = new();
        public string? CouponCode { get; set; }
    }

    public class CalculateDiscountItem
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string CategoryId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
