namespace SocialListening.Startup;

using Listeners;
using Services;
using Tasks;

internal static class ServiceExtensions
{
    internal static IServiceCollection AddMyServices(this IServiceCollection services)
    {
        services.AddHostedService<PollRedditTask>();
        services.AddSingleton<RedditStatisticsService>();
        services.AddSingleton<RedditListener>();
        return services;
    }
}