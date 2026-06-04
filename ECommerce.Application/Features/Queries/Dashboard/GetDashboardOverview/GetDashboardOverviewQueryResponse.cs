namespace ECommerce.Application.Features.Queries.Dashboard.GetDashboardOverview
{
    public class GetDashboardOverviewQueryResponse
    {
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public int TotalUsers { get; set; }
    }
}
