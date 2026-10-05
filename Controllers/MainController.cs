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
    public ActionResult<List<MemberDTO>> GetLeaderboard()
    {
        var result = _leaderboardService.GetTop(20);
        return Ok(result);
    }

    [HttpGet("katas")]
    public ActionResult<List<KataDTO>> GetKatas()
    {
        return _leaderboardService.GetKatas();
    }
    
}
