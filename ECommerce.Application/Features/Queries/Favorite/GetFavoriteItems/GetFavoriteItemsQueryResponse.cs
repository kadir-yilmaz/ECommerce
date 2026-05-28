namespace ECommerce.Application.Features.Queries.Favorite.GetFavoriteItems
{
    public class GetFavoriteItemsQueryResponse
    {
        public string FavoriteItemId { get; set; }
        public string ProductId { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public int Stock { get; set; }
        public string? ImagePath { get; set; }
    }
}
