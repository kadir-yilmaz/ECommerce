using ECommerce.Domain.Enums;

namespace ECommerce.Application.DTOs.Address
{
    public class UpdateAddressDTO
    {
        public string Id { get; set; }
        public AddressType AddressType { get; set; }
        public AddressCategory Category { get; set; }
        public string? ClosedDays { get; set; }
        public string Title { get; set; }
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
