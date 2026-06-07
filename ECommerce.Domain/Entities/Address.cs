using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities
{
    public class Address : BaseEntity
    {
        // Kullanıcı ilişkisi
        public string UserId { get; set; }
        public AppUser User { get; set; }

        // Adres tipi (Teslimat veya Fatura)
        public AddressType AddressType { get; set; } = AddressType.Delivery;

        // Adres kategorisi (Ev veya İş yeri)
        public AddressCategory Category { get; set; } = AddressCategory.Home;

        // İş yeri için kapalı günler (virgülle ayrılmış: "Cumartesi,Pazar")
        public string? ClosedDays { get; set; }

        // Adres başlığı (Evim, İşim, vb.)
        public string Title { get; set; } = "Evim";

        // Varsayılan adres mi?
        public bool IsDefault { get; set; }

        // Kişi bilgileri
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

        // Konum bilgileri (hiyerarşik)
        public string Province { get; set; }       // İl
        public string District { get; set; }       // İlçe
        public string Neighborhood { get; set; }   // Mahalle
        public string PostalCode { get; set; }     // Posta Kodu

        // Açık adres
        public string AddressDetail { get; set; }
    }
}
