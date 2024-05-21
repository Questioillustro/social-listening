using Microsoft.AspNetCore.Mvc;

namespace SocialListening.Controllers;

using Responses;
using Services;

[ApiController]
[Route("[controller]")]
public class RedditStatistics : ControllerBase
{
    private readonly RedditStatisticsService _redditStatisticsService;

    public RedditStatistics(RedditStatisticsService redditStatisticsService)
    {
        _redditStatisticsService = redditStatisticsService;
    }

    [HttpGet(Name = "GetRedditPosts")]
    public RedditStatisticsResponse Get()
    {
        return _redditStatisticsService.GetRedditStatistics();
    }
}
