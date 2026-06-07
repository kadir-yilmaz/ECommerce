using ECommerce.Application.Abstractions.Services;
using ECommerce.Application.DTOs.Address;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressReadRepository _addressReadRepository;
        private readonly IAddressWriteRepository _addressWriteRepository;

        public AddressService(
            IAddressReadRepository addressReadRepository,
            IAddressWriteRepository addressWriteRepository)
        {
            _addressReadRepository = addressReadRepository;
            _addressWriteRepository = addressWriteRepository;
        }

        public async Task<List<AddressDTO>> GetUserAddressesAsync(string userId, AddressType? type = null)
        {
            var query = _addressReadRepository.GetWhere(a => a.UserId == userId);

            if (type.HasValue)
                query = query.Where(a => a.AddressType == type.Value);

            var addresses = await query
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.CreatedDate)
                .ToListAsync();

            return addresses.Select(a => new AddressDTO
            {
                Id = a.Id.ToString(),
                UserId = a.UserId,
                AddressType = a.AddressType,
                Category = a.Category,
                ClosedDays = a.ClosedDays,
                Title = a.Title,
                IsDefault = a.IsDefault,
                FirstName = a.FirstName,
                LastName = a.LastName,
                PhoneNumber = a.PhoneNumber,
                Province = a.Province,
                District = a.District,
                Neighborhood = a.Neighborhood,
                PostalCode = a.PostalCode,
                AddressDetail = a.AddressDetail
            }).ToList();
        }

        public async Task<AddressDTO?> GetAddressByIdAsync(string addressId, string userId)
        {
            var address = await _addressReadRepository
                .GetWhere(a => a.Id == Guid.Parse(addressId) && a.UserId == userId)
                .FirstOrDefaultAsync();

            if (address == null)
                return null;

            return new AddressDTO
            {
                Id = address.Id.ToString(),
                UserId = address.UserId,
                AddressType = address.AddressType,
                Category = address.Category,
                ClosedDays = address.ClosedDays,
                Title = address.Title,
                IsDefault = address.IsDefault,
                FirstName = address.FirstName,
                LastName = address.LastName,
                PhoneNumber = address.PhoneNumber,
                Province = address.Province,
                District = address.District,
                Neighborhood = address.Neighborhood,
                PostalCode = address.PostalCode,
                AddressDetail = address.AddressDetail
            };
        }

        public async Task<AddressDTO> CreateAddressAsync(CreateAddressDTO createAddressDTO, string userId)
        {
            // Eğer bu varsayılan adres olarak işaretleniyorsa, aynı tipteki diğer adreslerin varsayılan flag'ini kaldır
            if (createAddressDTO.IsDefault)
            {
                await ClearDefaultFlagsAsync(userId, createAddressDTO.AddressType);
            }

            var address = new Address
            {
                UserId = userId,
                AddressType = createAddressDTO.AddressType,
                Category = createAddressDTO.Category,
                ClosedDays = createAddressDTO.ClosedDays,
                Title = createAddressDTO.Title,
                IsDefault = createAddressDTO.IsDefault,
                FirstName = createAddressDTO.FirstName,
                LastName = createAddressDTO.LastName,
                PhoneNumber = createAddressDTO.PhoneNumber,
                Province = createAddressDTO.Province,
                District = createAddressDTO.District,
                Neighborhood = createAddressDTO.Neighborhood,
                PostalCode = createAddressDTO.PostalCode,
                AddressDetail = createAddressDTO.AddressDetail
            };

            await _addressWriteRepository.AddAsync(address);
            await _addressWriteRepository.SaveAsync();

            return new AddressDTO
            {
                Id = address.Id.ToString(),
                UserId = address.UserId,
                AddressType = address.AddressType,
                Category = address.Category,
                ClosedDays = address.ClosedDays,
                Title = address.Title,
                IsDefault = address.IsDefault,
                FirstName = address.FirstName,
                LastName = address.LastName,
                PhoneNumber = address.PhoneNumber,
                Province = address.Province,
                District = address.District,
                Neighborhood = address.Neighborhood,
                PostalCode = address.PostalCode,
                AddressDetail = address.AddressDetail
            };
        }

        public async Task<AddressDTO> UpdateAddressAsync(UpdateAddressDTO updateAddressDTO, string userId)
        {
            var address = await _addressReadRepository
                .GetWhere(a => a.Id == Guid.Parse(updateAddressDTO.Id) && a.UserId == userId)
                .FirstOrDefaultAsync();

            if (address == null)
                throw new UnauthorizedAccessException("Bu adresi düzenleme yetkiniz yok.");

            // Eğer varsayılan adres olarak işaretleniyorsa, aynı tipteki diğer adreslerin varsayılan flag'ini kaldır
            if (updateAddressDTO.IsDefault && !address.IsDefault)
            {
                await ClearDefaultFlagsAsync(userId, updateAddressDTO.AddressType);
            }

            address.AddressType = updateAddressDTO.AddressType;
            address.Category = updateAddressDTO.Category;
            address.ClosedDays = updateAddressDTO.ClosedDays;
            address.Title = updateAddressDTO.Title;
            address.IsDefault = updateAddressDTO.IsDefault;
            address.FirstName = updateAddressDTO.FirstName;
            address.LastName = updateAddressDTO.LastName;
            address.PhoneNumber = updateAddressDTO.PhoneNumber;
            address.Province = updateAddressDTO.Province;
            address.District = updateAddressDTO.District;
            address.Neighborhood = updateAddressDTO.Neighborhood;
            address.PostalCode = updateAddressDTO.PostalCode;
            address.AddressDetail = updateAddressDTO.AddressDetail;

            _addressWriteRepository.Update(address);
            await _addressWriteRepository.SaveAsync();

            return new AddressDTO
            {
                Id = address.Id.ToString(),
                UserId = address.UserId,
                AddressType = address.AddressType,
                Category = address.Category,
                ClosedDays = address.ClosedDays,
                Title = address.Title,
                IsDefault = address.IsDefault,
                FirstName = address.FirstName,
                LastName = address.LastName,
                PhoneNumber = address.PhoneNumber,
                Province = address.Province,
                District = address.District,
                Neighborhood = address.Neighborhood,
                PostalCode = address.PostalCode,
                AddressDetail = address.AddressDetail
            };
        }

        public async Task DeleteAddressAsync(string addressId, string userId)
        {
            var address = await _addressReadRepository
                .GetWhere(a => a.Id == Guid.Parse(addressId) && a.UserId == userId)
                .FirstOrDefaultAsync();

            if (address == null)
                throw new UnauthorizedAccessException("Bu adresi silme yetkiniz yok.");

            _addressWriteRepository.Remove(address);
            await _addressWriteRepository.SaveAsync();
        }

        public async Task SetAsDefaultAsync(string addressId, string userId, AddressType type)
        {
            var address = await _addressReadRepository
                .GetWhere(a => a.Id == Guid.Parse(addressId) && a.UserId == userId)
                .FirstOrDefaultAsync();

            if (address == null)
                throw new UnauthorizedAccessException("Bu adresi varsayılan yapma yetkiniz yok.");

            // Önce aynı tipteki tüm adreslerin varsayılan flag'ini kaldır
            await ClearDefaultFlagsAsync(userId, type);

            // Seçili adresi varsayılan yap
            address.IsDefault = true;
            _addressWriteRepository.Update(address);
            await _addressWriteRepository.SaveAsync();
        }

        private async Task ClearDefaultFlagsAsync(string userId, AddressType type)
        {
            var defaultAddresses = await _addressReadRepository
                .GetWhere(a => a.UserId == userId && a.AddressType == type && a.IsDefault)
                .ToListAsync();

            foreach (var addr in defaultAddresses)
            {
                addr.IsDefault = false;
                _addressWriteRepository.Update(addr);
            }

            if (defaultAddresses.Any())
                await _addressWriteRepository.SaveAsync();
        }
    }
}
