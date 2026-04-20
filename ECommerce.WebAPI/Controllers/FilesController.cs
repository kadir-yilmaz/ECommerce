using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class FilesController : ControllerBase
    {
        [HttpGet("[action]")]
        public IActionResult GetBaseStorageUrl()
        {
            var runtimeBaseUrl = $"{Request.Scheme}://{Request.Host}";

            return Ok(new
            {
                Url = runtimeBaseUrl
            });
        }

    }
}
