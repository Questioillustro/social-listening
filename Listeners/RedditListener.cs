namespace SocialListening.Listeners;

using System.Net.Http.Headers;
using System.Text;
using Responses;

/*
 * TODO:
 * 1. Move sensitive values to .ENV and remove from code base
 * 2. Use a library for managing polling or change to websockets (reddit docs suggest this can be done)
 * 3. If polling is kept, extract rate limit and adjust polling to maximize the polling rate
 */
public static class RedditListener
{
    private static readonly Dictionary<string, RedditPost> PostMap = new();
    
    public static Dictionary<string, RedditPost> GetPostMap()
    {
        return PostMap;
    }

    public static async Task Poll(CancellationToken cancellationToken)
    {
        var authToken = await GetAuthToken();
        await PollReddit(authToken, cancellationToken);
    }

    private static async Task PollReddit(string authToken, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var jokes = await GetNewPostsForSubreddit("jokes", authToken);
            MapPosts(jokes);
            await Task.Delay(5000, cancellationToken);
        }
    }

    private static void MapPosts(List<RedditPostDataWrapper> wrappedJokes)
    {
        wrappedJokes.ForEach(wj => PostMap.Add(wj.data.id, wj.data));
    }

    private static async Task<string> GetAuthToken()
    {
        using var client = new HttpClient();

        var credentialsBase64 = Convert.ToBase64String(Encoding.ASCII.GetBytes("-ZmRQpmvaRS3HqrQhmXfYw:jMnFcby1i0xFHzFGFFilTopu6V8--Q"));
        
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://www.reddit.com/api/v1/access_token?grant_type=client_credentials");
        requestMessage.Headers.Authorization = 
            new AuthenticationHeaderValue("Basic", credentialsBase64);
        requestMessage.Headers.Add("User-Agent", "sociallistener v1 (by /u/CurrentDig9909)");
        
        var tokenResponse = await client.SendAsync(requestMessage);

        var token = await tokenResponse.Content.ReadFromJsonAsync<RedditAuthTokenResponse>();
        
        var accessToken = token!.access_token;

        if (accessToken == null) throw new Exception("No auth token available");
        
        return accessToken;
    }
    

    private static async Task<List<RedditPostDataWrapper>> GetNewPostsForSubreddit(string subreddit, string? token)
    {
        using var client = new HttpClient();
        
        var url = $"https://oauth.reddit.com/r/{subreddit}/new?limit=100";
        
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
        requestMessage.Headers.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
        requestMessage.Headers.Add("User-Agent", "sociallistener v1 (by /u/CurrentDig9909)");
        
        var response = await client.SendAsync(requestMessage);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadFromJsonAsync<SubredditListingResponse>() ?? null;

            if (content is null) throw new Exception("Null content response from Reddit");
            
            return content.data.children;
        }
        
        throw new HttpRequestException($"Error retrieving posts: {response.StatusCode}");
    }
}