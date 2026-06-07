using ECommerce.Domain.Enums;

namespace ECommerce.Application.DTOs.Address
{
    public class CreateAddressDTO
    {
        public AddressType AddressType { get; set; } = AddressType.Delivery;
        public AddressCategory Category { get; set; } = AddressCategory.Home;
        public string? ClosedDays { get; set; }
        public string Title { get; set; } = "Evim";
        public bool IsDefault { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Neighborhood { get; set; }
        public string PostalCode { get; set; }
        public string AddressDetail { get; set; }
    }
}
