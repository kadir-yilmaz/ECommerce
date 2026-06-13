using MediatR;
using System;

namespace ECommerce.Application.Features.Commands.ProductReview.ModerateReview
{
    public class ModerateReviewCommandRequest : IRequest<ModerateReviewCommandResponse>
    {
        public Guid ReviewId { get; set; }

        /// <summary>
        /// 1=Approved, 2=Rejected
        /// </summary>
        public int Status { get; set; }

        public string? AdminNote { get; set; }
    }
}
