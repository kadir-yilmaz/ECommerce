using ECommerce.Application.Abstractions.Services;
using ECommerce.Application.Consts;
using ECommerce.Application.CustomAttributes;
using ECommerce.Application.DTOs.Address;
using ECommerce.Application.Enums;
using ECommerce.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressesController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Addresses, ActionType = ActionType.Reading, Definition = "Get User Addresses")]
        public async Task<IActionResult> GetUserAddresses([FromQuery] AddressType? type = null)
        {
            var userId = GetUserId();
            var addresses = await _addressService.GetUserAddressesAsync(userId, type);
            return Ok(addresses);
        }

        [HttpGet("{id}")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Addresses, ActionType = ActionType.Reading, Definition = "Get Address By Id")]
        public async Task<IActionResult> GetAddressById(string id)
        {
            var userId = GetUserId();
            var address = await _addressService.GetAddressByIdAsync(id, userId);
            
            if (address == null)
                return NotFound(new { message = "Adres bulunamadı." });

            return Ok(address);
        }

        [HttpPost]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Addresses, ActionType = ActionType.Writing, Definition = "Create Address")]
        public async Task<IActionResult> CreateAddress([FromBody] CreateAddressDTO createAddressDTO)
        {
            var userId = GetUserId();
            var address = await _addressService.CreateAddressAsync(createAddressDTO, userId);
            return CreatedAtAction(nameof(GetAddressById), new { id = address.Id }, address);
        }

        [HttpPut("{id}")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Addresses, ActionType = ActionType.Updating, Definition = "Update Address")]
        public async Task<IActionResult> UpdateAddress(string id, [FromBody] UpdateAddressDTO updateAddressDTO)
        {
            if (id != updateAddressDTO.Id)
                return BadRequest(new { message = "ID uyuşmazlığı." });

            try
            {
                var userId = GetUserId();
                var address = await _addressService.UpdateAddressAsync(updateAddressDTO, userId);
                return Ok(address);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Addresses, ActionType = ActionType.Deleting, Definition = "Delete Address")]
        public async Task<IActionResult> DeleteAddress(string id)
        {
            try
            {
                var userId = GetUserId();
                await _addressService.DeleteAddressAsync(id, userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpPost("{id}/set-default")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Addresses, ActionType = ActionType.Updating, Definition = "Set Default Address")]
        public async Task<IActionResult> SetDefaultAddress(string id, [FromQuery] AddressType type)
        {
            try
            {
                var userId = GetUserId();
                await _addressService.SetAsDefaultAsync(id, userId, type);
                return Ok(new { message = "Varsayılan adres güncellendi." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }
    }
}
