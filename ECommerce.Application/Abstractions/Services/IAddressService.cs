using ECommerce.Application.DTOs.Address;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.Abstractions.Services
{
    public interface IAddressService
    {
        Task<List<AddressDTO>> GetUserAddressesAsync(string userId, AddressType? type = null);
        Task<AddressDTO?> GetAddressByIdAsync(string addressId, string userId);
        Task<AddressDTO> CreateAddressAsync(CreateAddressDTO createAddressDTO, string userId);
        Task<AddressDTO> UpdateAddressAsync(UpdateAddressDTO updateAddressDTO, string userId);
        Task DeleteAddressAsync(string addressId, string userId);
        Task SetAsDefaultAsync(string addressId, string userId, AddressType type);
    }
}
