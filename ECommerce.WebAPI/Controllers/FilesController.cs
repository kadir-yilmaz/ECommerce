using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class FilesController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public FilesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("[action]")]
        public IActionResult GetBaseStorageUrl()
        {
            var baseUrl = _configuration["BaseStorageUrl"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                baseUrl = $"{Request.Scheme}://{Request.Host}";
            }

            return Ok(new
            {
                Url = baseUrl
            });
        }

    }
}
