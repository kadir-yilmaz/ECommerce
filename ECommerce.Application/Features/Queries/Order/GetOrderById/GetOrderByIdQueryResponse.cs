using ECommerce.Application.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.Order.GetOrderById
{
    public class GetOrderByIdQueryResponse
    {
        public string Id { get; set; }
        public string Address { get; set; }
        public object BasketItems { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
        public string OrderCode { get; set; }
        public bool Completed { get; set; }
        public int Status { get; set; }
        public string? CargoCompany { get; set; }
        public string? TrackingNumber { get; set; }
        public decimal BasePrice { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalPrice { get; set; }
        public List<SingleOrderDiscount> OrderDiscounts { get; set; }
    }
}
