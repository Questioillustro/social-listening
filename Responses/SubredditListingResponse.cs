namespace SocialListening.Responses;

public record SubredditListingResponse(SubredditListingData data);

public record SubredditListingData(List<RedditPostDataWrapper> children);

public record RedditPostDataWrapper(RedditPost data);

public class RedditPost
{
    public string author { get; set; } = string.Empty;
    public int ups { get; set; } = 0;
    
    public string permalink { get; set; } = string.Empty;
    
    public string id { get; set; } = string.Empty;
}