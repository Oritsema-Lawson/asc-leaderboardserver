using System.Security.Claims;
using APCServer.DTO;
using APCServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace APCServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILoginService _loginService;

    public AuthController(ILoginService loginService)
    {
        _loginService = loginService;
    }

    [HttpPost("login")]
    public async Task<ActionResult> SignIn([FromBody] UserAuthDTO request)
    {
        var result = await _loginService.CheckLoginAsync(request.UserEmail, request.Password);
        
        if(result.res == LoginResult.IncorrectPassword || result.res == LoginResult.UnknownUser)
            return Unauthorized("Incorrect email or password.");

        if(result.permissions == "UNSPECIFIED")
            return Unauthorized("Account not yet active.");

        List<Claim> claims = new List<Claim>()
        {
          new Claim(ClaimTypes.Email, request.UserEmail),
          new Claim(ClaimTypes.Role, result.permissions)  
        };

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
        return Ok("Login Successful");
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] UserAuthDTO request)
    {
        var result = await _loginService.RegisterAsync(request.UserEmail, request.Password);
        
        if(result == LoginResult.UserExists)
            return Conflict("A user already exists with this email.");

        return Ok();
    }


    [HttpPost("logout")]
    public async Task<ActionResult> LogOut()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok("Logout Successful.");
    }
}