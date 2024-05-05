namespace SocialListening.Tasks;

using Listeners;

public class PollRedditTask : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        RedditListener.Poll(cancellationToken);
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}