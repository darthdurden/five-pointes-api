using FivePointes.Logic.Ports;
using FivePointes.Logic.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FivePointes.Logic
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<ITransactionsService, TransactionsService>();
            services.AddScoped<ITransactionCategoriesService, TransactionCategoriesService>();
            services.AddScoped<IAccountsService, AccountsService>();

            return services;
        }
    }
}
