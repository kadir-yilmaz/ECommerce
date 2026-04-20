using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class AppUser : IdentityUser<string>
    {
        public string NameSurname { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenEndDate { get; set; }

        // A user can have multiple historical baskets, but only one active basket without an order.
        public ICollection<Basket> Baskets { get; set; } = new List<Basket>();
    }
}
