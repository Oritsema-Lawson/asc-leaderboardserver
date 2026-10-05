using Microsoft.AspNetCore.Mvc;
using LeaderboardServer.Services;
using LeaderboardServer.DTO;


namespace LeaderboardServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private ILeaderboardService _leaderboardService;

    public AdminController(ILeaderboardService lbService)
    {
        _leaderboardService = lbService;
    }

    [HttpPost("users")]
    public ActionResult<List<MemberDTO>> AddUser()
    {
        return null;
    }

    [HttpPost("katas")]
    public ActionResult<List<MemberDTO>> AddKata()
    {
        return null;
    }

    [HttpDelete("usertrack")]
    public ActionResult<List<MemberDTO>> DeleteUser()
    {
        return null;
    }
    
    [HttpDelete("katatrack")]
    public ActionResult<List<MemberDTO>> DeleteKata()
    {
        return null;
    }
}
