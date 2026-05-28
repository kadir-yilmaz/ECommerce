using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.Order.CreateOrder
{
    public class CreateOrderCommandResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
