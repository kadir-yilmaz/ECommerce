using System.Threading.Tasks;

namespace ECommerce.Application.Abstractions.Discount
{
    public interface IRewardService
    {
        /// <summary>
        /// Kullanıcının son alışverişlerini kontrol eder ve ödül kazanıp kazanmadığını belirler.
        /// Kazandıysa otomatik kupon oluşturur ve UserCoupon'a atar.
        /// </summary>
        Task CheckAndGrantRewardsAsync(string userId);
    }
}
