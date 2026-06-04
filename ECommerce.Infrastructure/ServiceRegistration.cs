using ECommerce.Application.Abstractions.Services;
using ECommerce.Application.Abstractions.Services.Configurations;
using ECommerce.Application.Abstractions.Storage;
using ECommerce.Application.Abstractions.Token;
using ECommerce.Infrastructure.Services;
using ECommerce.Infrastructure.Services.Configurations;
using ECommerce.Infrastructure.Services.Storage;
using ECommerce.Infrastructure.Services.Token;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IStorageService, StorageService>();
            serviceCollection.AddScoped<ITokenHandler, TokenHandler>();
            serviceCollection.AddScoped<IMailService, MailService>();
            serviceCollection.AddScoped<IApplicationService, ApplicationService>();
            serviceCollection.AddScoped<IQRCodeService, QRCodeService>();
            serviceCollection.AddScoped<IPaymentService, PaymentService>();

            serviceCollection.AddScoped<ECommerce.Application.Abstractions.Discount.ICampaignRule, ECommerce.Application.Rules.DiscountRules.TotalAmountRule>();
            serviceCollection.AddScoped<ECommerce.Application.Abstractions.Discount.ICampaignRule, ECommerce.Application.Rules.DiscountRules.BuyXGetYFreeRule>();
            serviceCollection.AddScoped<ECommerce.Application.Abstractions.Discount.ICampaignRule, ECommerce.Application.Rules.DiscountRules.CheapestItemDiscountRule>();
            serviceCollection.AddScoped<ECommerce.Application.Abstractions.Discount.ICampaignRule, ECommerce.Application.Rules.DiscountRules.FreeShippingRule>();
            serviceCollection.AddScoped<ECommerce.Application.Abstractions.Discount.ICampaignRule, ECommerce.Application.Rules.DiscountRules.BrandDiscountRule>();
            serviceCollection.AddScoped<ECommerce.Application.Abstractions.Discount.ICampaignRule, ECommerce.Application.Rules.DiscountRules.CategoryDiscountRule>();
            serviceCollection.AddScoped<ECommerce.Application.Abstractions.Discount.ICampaignRule, ECommerce.Application.Rules.DiscountRules.SelectedProductsDiscountRule>();
            serviceCollection.AddScoped<ECommerce.Application.Abstractions.Discount.IDiscountService, ECommerce.Infrastructure.Services.DiscountService>();
            serviceCollection.AddScoped<ECommerce.Application.Abstractions.Discount.IRewardService, ECommerce.Infrastructure.Services.RewardService>();
        }
        public static void AddStorage<T>(this IServiceCollection serviceCollection) where T : StorageBase, IStorage
        {
            serviceCollection.AddScoped<IStorage, T>();
        }
    }
}
