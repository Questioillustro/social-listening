using Microsoft.AspNetCore.Mvc;

namespace SocialListening.Controllers;

using Responses;
using Services;

[ApiController]
[Route("[controller]")]
public class RedditStatistics : ControllerBase
{
    [HttpGet(Name = "GetRedditPosts")]
    public RedditStatisticsResponse Get()
    {
        return RedditStatisticsService.GetRedditStatistics();
    }
}
