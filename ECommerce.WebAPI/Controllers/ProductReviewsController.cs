using ECommerce.Application.Consts;
using ECommerce.Application.CustomAttributes;
using ECommerce.Application.Enums;
using ECommerce.Application.Features.Commands.ProductReview.CreateReview;
using ECommerce.Application.Features.Commands.ProductReview.DeleteReview;
using ECommerce.Application.Features.Commands.ProductReview.ModerateReview;
using ECommerce.Application.Features.Commands.ProductReview.UpdateReview;
using ECommerce.Application.Features.Commands.ProductReview.ToggleReviewReaction;
using ECommerce.Application.Features.Queries.ProductReview.CanUserReview;
using ECommerce.Application.Features.Queries.ProductReview.GetAllReviewsForAdmin;
using ECommerce.Application.Features.Queries.ProductReview.GetProductReviews;
using ECommerce.Application.Features.Queries.ProductReview.GetUserReview;
using ECommerce.Application.Features.Queries.ProductReview.GetUserReviews;
using ECommerce.Application.Features.Queries.ProductReview.GetUnreviewedProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductReviewsController : ControllerBase
    {
        readonly IMediator _mediator;

        public ProductReviewsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Onaylı yorumları sayfalanmış listeler (herkese açık)
        /// </summary>
        [HttpGet("{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductReviews([FromRoute] Guid productId, [FromQuery] int page = 0, [FromQuery] int size = 10)
        {
            var response = await _mediator.Send(new GetProductReviewsQueryRequest
            {
                ProductId = productId,
                Page = page,
                Size = size
            });
            return Ok(response);
        }

        /// <summary>
        /// Kullanıcının kendi yorumunu getirir
        /// </summary>
        [HttpGet("user/{productId}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        public async Task<IActionResult> GetUserReview([FromRoute] Guid productId)
        {
            var response = await _mediator.Send(new GetUserReviewQueryRequest { ProductId = productId });
            return Ok(response);
        }

        /// <summary>
        /// Kullanıcı yorum yapabilir mi kontrolü
        /// </summary>
        [HttpGet("can-review/{productId}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        public async Task<IActionResult> CanUserReview([FromRoute] Guid productId)
        {
            var response = await _mediator.Send(new CanUserReviewQueryRequest { ProductId = productId });
            return Ok(response);
        }

        /// <summary>
        /// Yeni yorum oluşturur
        /// </summary>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Admin")]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewCommandRequest request)
        {
            var response = await _mediator.Send(request);
            return StatusCode((int)HttpStatusCode.Created, response);
        }

        /// <summary>
        /// Kullanıcı kendi yorumunu günceller
        /// </summary>
        [HttpPut]
        [Authorize(AuthenticationSchemes = "Admin")]
        public async Task<IActionResult> UpdateReview([FromBody] UpdateReviewCommandRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        /// <summary>
        /// Kullanıcı kendi yorumunu siler (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        public async Task<IActionResult> DeleteReview([FromRoute] Guid id)
        {
            var response = await _mediator.Send(new DeleteReviewCommandRequest { ReviewId = id });
            return Ok(response);
        }

        /// <summary>
        /// Admin: Tüm yorumları listeler (filtre + flag bilgileri)
        /// </summary>
        [HttpGet("admin/all")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.ProductReviews, ActionType = ActionType.Reading, Definition = "Get All Reviews")]
        public async Task<IActionResult> GetAllReviewsForAdmin([FromQuery] GetAllReviewsForAdminQueryRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        /// <summary>
        /// Admin: Yorumu onaylar veya reddeder
        /// </summary>
        [HttpPut("admin/moderate")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.ProductReviews, ActionType = ActionType.Updating, Definition = "Moderate Review")]
        public async Task<IActionResult> ModerateReview([FromBody] ModerateReviewCommandRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        /// <summary>
        /// Kullanıcının yaptığı tüm yorumları (silinmişler dahil) listeler
        /// </summary>
        [HttpGet("my-reviews")]
        [Authorize(AuthenticationSchemes = "Admin")]
        public async Task<IActionResult> GetUserReviews()
        {
            var response = await _mediator.Send(new GetUserReviewsQueryRequest());
            return Ok(response);
        }

        /// <summary>
        /// Kullanıcının teslim edilmiş ama henüz yorum yapmadığı ürünleri listeler
        /// </summary>
        [HttpGet("unreviewed")]
        [Authorize(AuthenticationSchemes = "Admin")]
        public async Task<IActionResult> GetUnreviewedProducts()
        {
            var response = await _mediator.Send(new GetUnreviewedProductsQueryRequest());
            return Ok(response);
        }

        /// <summary>
        /// Yorum beğenme/beğenmeme (like/dislike) reaksiyonunu tetikler (toggle)
        /// </summary>
        [HttpPost("react")]
        [Authorize(AuthenticationSchemes = "Admin")]
        public async Task<IActionResult> ToggleReviewReaction([FromBody] ToggleReviewReactionCommandRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}
