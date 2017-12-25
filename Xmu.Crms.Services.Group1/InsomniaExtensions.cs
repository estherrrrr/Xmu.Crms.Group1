using Xmu.Crms.Services.Insomnia;
using Xmu.Crms.Shared.Service;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InsomniaExtensions
    {
        public static IServiceCollection AddInsomniaSeminarGroupService(this IServiceCollection serviceCollection) =>
            serviceCollection.AddScoped<ISeminarGroupService, SeminarGroupService>();

        public static IServiceCollection AddInsomniaFixedGroupService(this IServiceCollection serviceCollection) =>
            serviceCollection.AddScoped<IFixGroupService, FixedGroupService>();


        public static IServiceCollection AddInsomniaLoginService(this IServiceCollection serviceCollection) =>
            serviceCollection.AddScoped<ILoginService, LoginService>();

    }
}