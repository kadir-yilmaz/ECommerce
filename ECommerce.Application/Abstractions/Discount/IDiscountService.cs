using ECommerce.Application.DTOs.Discount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Abstractions.Discount
{
    public interface IDiscountService
    {
        Task<CalculateDiscountResponse> CalculateDiscountAsync(CalculateDiscountRequest request);
    }
}
