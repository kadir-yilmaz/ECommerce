namespace ECommerce.Domain.Enums
{
    public enum ReviewStatus
    {
        /// <summary>
        /// Yorum onay bekliyor
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Admin tarafından onaylandı
        /// </summary>
        Approved = 1,

        /// <summary>
        /// Admin tarafından reddedildi
        /// </summary>
        Rejected = 2
    }
}
