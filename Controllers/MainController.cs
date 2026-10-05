using Microsoft.AspNetCore.Mvc;
using LeaderboardServer.Services;
using LeaderboardServer.DTO;


namespace LeaderboardServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MainController : ControllerBase
{
    private ILeaderboardService _leaderboardService;

    public MainController(ILeaderboardService lbService)
    {
        _leaderboardService = lbService;
    }

    [HttpGet("leaderboard")]
    public async Task<ActionResult<List<MemberDTO>>> GetLeaderboard()
    {
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
