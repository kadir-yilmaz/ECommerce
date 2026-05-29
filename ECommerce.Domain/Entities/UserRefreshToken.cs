using System;

namespace ECommerce.Domain.Entities
{
    public class UserRefreshToken : BaseEntity
    {
        public string UserId { get; set; }
        public AppUser User { get; set; }

        public string Token { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime? RevokedDate { get; set; }
    }
}
