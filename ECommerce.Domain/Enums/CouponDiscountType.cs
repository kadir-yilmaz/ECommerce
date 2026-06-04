namespace ECommerce.Domain.Enums
{
    public enum CouponDiscountType
    {
        /// <summary>
        /// Sabit tutar indirimi (Örn: 50 TL)
        /// </summary>
        Amount = 0,

        /// <summary>
        /// Yüzdelik indirim (Örn: %15, max 200 TL)
        /// </summary>
        Percentage = 1,

        /// <summary>
        /// Kargo ücretsiz kuponu
        /// </summary>
        FreeShipping = 2
    }
}
