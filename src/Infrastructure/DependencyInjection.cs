using Galliard.Application.Common.Interfaces;
using Galliard.Domain.Constants;
using Galliard.Infrastructure.Data;
using Galliard.Infrastructure.Data.Interceptors;
using Galliard.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        // builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        // builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        //
        // builder.Services
        //     .AddDefaultIdentity<ApplicationUser>()
        //     .AddRoles<IdentityRole>();
        //
        // builder.Services.AddSingleton(TimeProvider.System);
        // builder.Services.AddTransient<IIdentityService, IdentityService>();
        //
        // builder.Services.AddAuthorization(options =>
        //     options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator)));
    }
}
