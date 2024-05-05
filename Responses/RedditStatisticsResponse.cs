namespace SocialListening.Responses;

public class RedditStatisticsResponse
{
    public KeyValuePair<string, int> MostActiveUser { get; set; }

    public RedditPost MostUpvotedPost { get; set; } = new();
}