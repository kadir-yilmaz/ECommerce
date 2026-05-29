using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities
{
    public class AppUser : IdentityUser<string>
    {
        public string NameSurname { get; set; }

        public ICollection<UserRefreshToken> UserRefreshTokens { get; set; } = new List<UserRefreshToken>();

        // A user can have multiple historical baskets, but only one active basket without an order.
        public ICollection<Basket> Baskets { get; set; } = new List<Basket>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}
