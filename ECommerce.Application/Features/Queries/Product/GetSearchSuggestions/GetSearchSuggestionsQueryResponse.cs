namespace ECommerce.Application.Features.Queries.Product.GetSearchSuggestions
{
    public class GetSearchSuggestionsQueryResponse
    {
        public string Text { get; set; }
        public string Type { get; set; } // "Marka", "Kategori", "KategoriCombo", "Ürün"
        public string? TargetId { get; set; }
    }
}
