using System;

namespace ECommerce.Domain.Entities
{
    public class RewardRule : BaseEntity
    {
        /// <summary>
        /// Kural adı (Örn: "2000 TL Alışverişe 100 TL İndirim")
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Süre (gün olarak). Örn: 30 gün içinde
        /// </summary>
        public int PeriodInDays { get; set; }

        /// <summary>
        /// Minimum alışveriş tutarı (TL). Örn: 2000 TL
        /// </summary>
        public decimal MinTotalSpent { get; set; }

        /// <summary>
        /// İndirim Tipi: Amount, Percentage, FreeShipping
        /// </summary>
        public string RewardDiscountType { get; set; } = "Amount";

        /// <summary>
        /// Kazanılacak kupon indirim değeri (Yüzde veya Tutar)
        /// </summary>
        public decimal RewardDiscountValue { get; set; }

        /// <summary>
        /// Yüzdelik indirimler için maksimum indirim tutarı (Opsiyonel)
        /// </summary>
        public decimal? RewardMaxDiscountAmount { get; set; }

        /// <summary>
        /// Ödül kuponunun geçerlilik süresi (gün). Örn: 30 gün
        /// </summary>
        public int CouponValidityDays { get; set; }

        /// <summary>
        /// Ödül kuponunun minimum sepet tutarı
        /// </summary>
        public decimal CouponMinCartAmount { get; set; }

        public bool IsActive { get; set; }
    }
}
