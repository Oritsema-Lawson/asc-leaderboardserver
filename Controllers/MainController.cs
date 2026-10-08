using Microsoft.AspNetCore.Mvc;
using APCServer.Services;
using APCServer.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.UI.Services;


namespace APCServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MainController : ControllerBase
{
    private readonly ILeaderboardService _leaderboardService;
    private readonly ILogger<MainController> _logger;

    public MainController(ILeaderboardService lbService, ILogger<MainController> logger)
    {
        _leaderboardService = lbService;
        _logger = logger;
    }

    [HttpGet("leaderboard")]
    public async Task<ActionResult<List<MemberDTO>>> GetLeaderboard()
    {
        _logger.LogInformation("Sending leaderboard.");
        var result = _leaderboardService.GetTop(20);
        return Ok(await result);
    }

    [HttpGet("katas")]
    public async Task<ActionResult<List<KataDTO>>> GetKatas()
    {
        var result = _leaderboardService.GetKatas();
        return Ok(await result);
    }   
}
