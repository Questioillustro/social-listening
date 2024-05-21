namespace SocialListening.Listeners;

using System.Net.Http.Headers;
using System.Text;
using Responses;
using Startup;

/*
 * TODO:
 * 1. Move sensitive values to .ENV and remove from code base
 * 2. Potentially change to websockets (reddit docs suggest this can be done)
 */
public class RedditListener
{
    private Dictionary<string, RedditPost> _postMap = new();
    private readonly RedditConfig _config;
    private int _pollRate = 5000;

    public RedditListener(RedditConfig config)
    {
        _config = config;
    }

    public Dictionary<string, RedditPost> GetPostMap()
    {
        return _postMap;
    }

    public async Task Poll(CancellationToken cancellationToken)
    {
        var authToken = await GetAuthToken();
        await PollReddit(authToken, cancellationToken);
    }

    private async Task PollReddit(string authToken, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var jokes = await GetNewPostsForSubreddit("jokes", authToken);
            MapPosts(jokes);
            await Task.Delay(_pollRate, cancellationToken);
        }
    }

    private void MapPosts(List<RedditPostDataWrapper> wrappedJokes)
    {
        _postMap = new();
        wrappedJokes.ForEach(wj => _postMap.TryAdd(wj.data.id, wj.data));
    }

    private async Task<string> GetAuthToken()
    {
        using var client = new HttpClient();

        var credentialsBase64 = Convert.ToBase64String(Encoding.ASCII.GetBytes(_config.Credentials));
        
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, _config.AuthUrl);
        requestMessage.Headers.Authorization = 
            new AuthenticationHeaderValue("Basic", credentialsBase64);
        requestMessage.Headers.Add("User-Agent", _config.UserAgent);
        
        var tokenResponse = await client.SendAsync(requestMessage);

        var token = await tokenResponse.Content.ReadFromJsonAsync<RedditAuthTokenResponse>();
        
        var accessToken = token!.access_token;

        if (accessToken == null) throw new Exception("No auth token available");
        
        return accessToken;
    }
    

    private async Task<List<RedditPostDataWrapper>> GetNewPostsForSubreddit(string subreddit, string? token)
    {
        using var client = new HttpClient();
        
        var url = $"{_config.BaseUrl}/r/{subreddit}/new?limit=100";
        
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
        requestMessage.Headers.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
        requestMessage.Headers.Add("User-Agent", _config.UserAgent);
        
        var response = await client.SendAsync(requestMessage);

        HttpHeaders headers = response.Headers;
        UpdateRate(headers);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadFromJsonAsync<SubredditListingResponse>() ?? null;

            if (content is null) throw new Exception("Null content response from Reddit");
            
            return content.data.children;
        }
        
        throw new HttpRequestException($"Error retrieving posts: {response.StatusCode}");
    }

    private void UpdateRate(HttpHeaders responseHeaders)
    {
        responseHeaders.TryGetValues("x-ratelimit-reset", out IEnumerable<string> rateLimitReset);
        responseHeaders.TryGetValues("x-ratelimit-remaining", out IEnumerable<string> rateRemaining);

        if (rateLimitReset == null || rateRemaining == null) return;
        
        var resetSeconds = double.Parse(rateLimitReset.FirstOrDefault("0"));
        var remaining = double.Parse(rateRemaining.FirstOrDefault("0"));

        _pollRate = (int)(resetSeconds * 1000 / remaining);
    }
}