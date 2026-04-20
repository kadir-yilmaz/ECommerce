namespace ECommerce.Application.Features.Queries.AppUser.GetUserProfile
{
    public class GetUserProfileQueryResponse
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NameSurname { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
    }
}
