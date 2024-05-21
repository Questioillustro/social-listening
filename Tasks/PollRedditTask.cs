namespace SocialListening.Tasks;

using Listeners;

public class PollRedditTask : BackgroundService
{
    private readonly RedditListener _redditListener;

    public PollRedditTask(RedditListener redditListener)
    {
        _redditListener = redditListener;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _redditListener.Poll(stoppingToken);
    }
}