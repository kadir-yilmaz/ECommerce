namespace ECommerce.Application.DTOs.Order
{
    public class SingleOrder
    {
        public string Address { get; set; }
        public object BasketItems { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
        public string Id { get; set; }
        public string OrderCode { get; set; }
        public bool Completed { get; set; }
        public int Status { get; set; }
        public string? CargoCompany { get; set; }
        public string? TrackingNumber { get; set; }

        /// <summary>
        /// Ham ürün toplamı (indirimler uygulanmadan)
        /// </summary>
        public decimal BasePrice { get; set; }

        /// <summary>
        /// Toplam indirim tutarı
        /// </summary>
        public decimal TotalDiscount { get; set; }

        /// <summary>
        /// Ödenecek net tutar (BasePrice - TotalDiscount)
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Uygulanmış indirim detayları (kampanya, kupon vb.)
        /// </summary>
        public List<SingleOrderDiscount> OrderDiscounts { get; set; } = new();
    }

    public class SingleOrderDiscount
    {
        public string DiscountName { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountAmount { get; set; }
    }
}

