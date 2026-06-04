using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.Discount
{
    public class CalculateDiscountResponse
    {
        public decimal OriginalTotal { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal FinalTotal { get; set; }
        public bool IsShippingFree { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal ShippingThreshold { get; set; }
        public List<DiscountDetail> AppliedDiscounts { get; set; } = new();
    }

    public class DiscountDetail
    {
        public string DiscountName { get; set; }
        public string DiscountType { get; set; } // Campaign, Coupon, FreeShipping vs.
        public decimal DiscountAmount { get; set; }
    }
}
