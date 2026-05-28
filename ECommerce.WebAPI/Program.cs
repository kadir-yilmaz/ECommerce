using ECommerce.Persistence;
using ECommerce.Persistence.Contexts;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using ECommerce.Application;
using ECommerce.Infrastructure;
using ECommerce.Infrastructure.Filters;
using FluentValidation.AspNetCore;
using ECommerce.Application.Validators.Products;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Serilog.Context;
using ECommerce.SignalR;
using ECommerce.SignalR.Hubs;
using ECommerce.WebAPI.Extensions;
using ECommerce.WebAPI.Filters;

using ECommerce.Application.Configurations;

DotNetEnv.Env.Load();
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("serilog.json", optional: false, reloadOnChange: true);

// Serilog Configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<IyzipaySettings>(builder.Configuration.GetSection("Iyzipay"));
builder.Services.AddPersistenceServices();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddSignalRServices();
builder.Services.AddStorage<ECommerce.Infrastructure.Services.Storage.Local.LocalStorage>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
    options.Filters.Add<RolePermissionFilter>();
})
    .AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<CreateProductValidator>())
    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.SetIsOriginAllowed(origin => 
    {
        var host = new Uri(origin).Host;
        return host == "localhost"
            || host == "kadir.infinityfreeapp.com"
            || host == "kadir.tryasp.net"
            || host.EndsWith(".trycloudflare.com");
    })
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials()
));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Admin";
    options.DefaultChallengeScheme = "Admin";
    options.DefaultScheme = "Admin";
})
    .AddJwtBearer("Admin", options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidAudience = builder.Configuration["Token:Audience"],
            ValidIssuer = builder.Configuration["Token:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"]!)),
            LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false,
        };
    });

var app = builder.Build();

app.ConfigureExceptionHandler<Program>(app.Services.GetRequiredService<ILogger<Program>>());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}
app.UseStaticFiles();
app.UseCors();

// app.UseHttpsRedirection();

app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
        diagnosticContext.Set("RequestPath", httpContext.Request.Path);
    };

    // 5xx → Error, 4xx → Warning, diğerleri → Information
    options.GetLevel = (httpContext, elapsed, ex) =>
    {
        if (ex != null || httpContext.Response.StatusCode >= 500)
            return Serilog.Events.LogEventLevel.Error;

        if (httpContext.Response.StatusCode >= 400)
            return Serilog.Events.LogEventLevel.Warning;

        return Serilog.Events.LogEventLevel.Information;
    };
});

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var username = context.User?.Identity?.IsAuthenticated == true ? context.User.Identity.Name : null;
    using (LogContext.PushProperty("user_name", username))
    {
        await next();
    }
});

app.MapControllers();
app.MapHub<ProductHub>("/products-hub");
app.MapHub<OrderHub>("/orders-hub");

// Seed data - DB boşsa 10 ürün ekle
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

    await DataSeeder.SeedCategoriesAsync(context);
    await DataSeeder.SeedProductsAsync(context);
    await DataSeeder.SeedRolesAndUsersAsync(userManager, roleManager, context);
}

app.Run();
