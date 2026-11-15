using ECommerce.Application.Features.Commands.Favorite.AddFavoriteItem;
using ECommerce.Application.Features.Commands.Favorite.RemoveFavoriteItem;
using ECommerce.Application.Features.Commands.Favorite.RemoveFavoriteItemByProductId;
using ECommerce.Application.Features.Queries.Favorite.GetFavoriteItems;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        readonly IMediator _mediator;

        public FavoritesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetFavoriteItems([FromQuery] GetFavoriteItemsQueryRequest request)
        {
            List<GetFavoriteItemsQueryResponse> response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddFavoriteItem(AddFavoriteItemCommandRequest request)
        {
            AddFavoriteItemCommandResponse response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpDelete("{FavoriteItemId}")]
        public async Task<IActionResult> RemoveFavoriteItem([FromRoute] RemoveFavoriteItemCommandRequest request)
        {
            RemoveFavoriteItemCommandResponse response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpDelete("product/{ProductId}")]
        public async Task<IActionResult> RemoveFavoriteItemByProductId([FromRoute] RemoveFavoriteItemByProductIdCommandRequest request)
        {
            RemoveFavoriteItemByProductIdCommandResponse response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}
