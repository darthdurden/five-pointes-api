using FivePointes.Api.Configuration;
using FivePointes.Logic.Ports;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NodaTime;

namespace FivePointes.Api.Adapters
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAdapters(this IServiceCollection services, IWebHostEnvironment env)
        {
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<ITransactionsRepository, TransactionsRepository>();
            services.AddScoped<ITransactionCategoriesRepository, TransactionCategoriesRepository>();
            services.AddScoped<IAccountsRepository, AccountsRepository>();
            services.AddScoped<IClientTimeEntriesRepository, ClockifyClientTimeEntriesRepository>();
            services.AddScoped<ITimeOffEntriesRepository, TimeOffEntriesRepository>();
            services.AddScoped<IClientsRepository, ClockifyClientsRepository>();
            services.AddScoped(serviceProvider => {
                var options = serviceProvider.GetRequiredService<IOptions<ClockifyOptions>>();
                return new Clockify.Net.ClockifyClient(options.Value.ApiKey);
            });
            services.AddScoped<IClock>(serviceProvider => SystemClock.Instance);
            //services.AddScoped<IClock>(ServiceProvider => new TestClock());

            return services;
        }
    }
}
