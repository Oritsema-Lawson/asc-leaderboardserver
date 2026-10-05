using Microsoft.AspNetCore.Mvc;
using LeaderboardServer.Services;
using LeaderboardServer.DTO;
using LeaderboardServer.Entities;


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
    public async Task<ActionResult> AddMember([FromBody] MemberDTO request)
    {
        int result = await _leaderboardService.AddMember(request.Username);

        if(result == 1)
            return Conflict("Member already exists.");

        return Ok("Member successfully added to tracking list.");
    }

    [HttpPost("katas")]
    public async Task<ActionResult> AddKata([FromBody] KataDTO request)
    {
        int result = await _leaderboardService.AddKata(request.KataRemoteId,
                                                        request.KataName,
                                                        request.KataUrl,
                                                        request.PointDifficulty);

        if(result == 1)
            return Conflict("Kata already exists.");

        return Ok("Kata successfully added to tracking list.");
    }

    [HttpDelete("users/{request}")]
    public async Task<ActionResult> DeleteMember(string request)
    {
        int result = await _leaderboardService.DelMember(request);

        if(result == 1)
            return NotFound("User not found.");

        return Ok("User successfully deleted.");

    }
    
    [HttpDelete("katas/{request}")]
    public async Task<ActionResult> DeleteKata(string request)
    {
        int result = await _leaderboardService.DelKata(request);

        if(result == 1)
            return NotFound("Kata not found.");

        return Ok("Kata successfully deleted.");
    }
}
