
namespace ECommerce.Domain.Entities
{
    public class EmailVerificationCode : BaseEntity
    {
        public string UserId { get; set; }
        public string Code { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        
        public AppUser User { get; set; }
    }
}
