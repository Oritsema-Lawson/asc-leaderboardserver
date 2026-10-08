using Microsoft.AspNetCore.Mvc;
using APCServer.Services;
using APCServer.DTO;
using APCServer.Entities;
using Microsoft.AspNetCore.Authorization;


namespace APCServer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly ILeaderboardService _leaderboardService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(ILeaderboardService lbService, ILogger<AdminController> logger)
    {
        _leaderboardService = lbService;
        _logger = logger;
    }

    [HttpPost("members")]
    public async Task<ActionResult> AddMember([FromBody] MemberDTO request)
    {
        _logger.LogInformation("Adding member {memberUsername} to tracking list.", request.Username);
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

    [HttpDelete("members/{request}")]
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
