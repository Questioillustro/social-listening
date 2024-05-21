namespace SocialListening.Services;

using Listeners;
using Responses;

public class RedditStatisticsService
{
    private readonly RedditListener _redditListener;

    public RedditStatisticsService(RedditListener redditListener)
    {
        _redditListener = redditListener;
    }

    public RedditStatisticsResponse GetRedditStatistics()
    {
        var posts = _redditListener.GetPostMap();
        var response = new RedditStatisticsResponse
        {
            MostUpvotedPost = GetMostUpvoted(posts.Values),
            MostActiveUser = GetMostActiveUser(posts.Values)
        };

        return response;
    }

    private static RedditPost GetMostUpvoted(IEnumerable<RedditPost> posts)
    {
        var mostUpvotes = 0;
        var mostUpvotedPost = new RedditPost();
        foreach (var redditPost in posts.Where(redditPost => redditPost.ups > mostUpvotes))
        {
            mostUpvotedPost = redditPost;
            mostUpvotes = redditPost.ups;
        }

        return mostUpvotedPost;
    }

    private static KeyValuePair<string, int> GetMostActiveUser(IEnumerable<RedditPost> posts)
    {
        var postCountMap = new Dictionary<string, int>();
        foreach (var redditPost in posts)
        {
            var count = postCountMap.GetValueOrDefault(redditPost.author, 0);
            postCountMap[redditPost.author] = ++count;
        }

        return postCountMap.MaxBy(kv => kv.Value);
    }
}