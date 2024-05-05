namespace SocialListening.Startup;

using Tasks;

internal static class ServiceExtensions
{
    internal static IServiceCollection AddMyServices(this IServiceCollection services)
    {
        services.AddHostedService<PollRedditTask>();
        return services;
    }
}