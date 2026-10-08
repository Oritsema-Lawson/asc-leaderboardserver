using APCServer.Data;
using APCServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace APCServer.Services;

public enum LoginResult : int
{
    Success = 0,
    IncorrectPassword = 1,
    UnknownUser = 2,
    UserExists = 3
}

public interface ILoginService
{
    public Task<(LoginResult res, string permissions)> CheckLoginAsync(string userEmail, string password);
    public Task<LoginResult> RegisterAsync(string userEmail, string password);
}

public class LoginService : ILoginService
{
    private readonly ApcDbContext _context;
    private readonly IHashService _hashService;

    public LoginService(ApcDbContext context, IHashService hashService)
    {
        _context = context;
        _hashService = hashService;
    }


    public async Task<(LoginResult res, string permissions)> CheckLoginAsync(string userEmail, string password)
    {
        User? user = await _context.UserAuth.FirstOrDefaultAsync(x => x.UserEmail == userEmail);

        if(user is null)
            return (LoginResult.UnknownUser, String.Empty);

        bool res = await _hashService.CompareAsync(password, user.PasswordHash);

        return res ? (LoginResult.Success, user.PermissionLevel)  : (LoginResult.IncorrectPassword, String.Empty);
    }


    public async Task<LoginResult> RegisterAsync(string userEmail, string password)
    {
        if(await _context.UserAuth.AnyAsync(x => x.UserEmail == userEmail))
            return LoginResult.UserExists;
        
        string passwordHash = await _hashService.HashAsync(password);
        
        await _context.UserAuth.AddAsync(new User
        {
            UserEmail = userEmail,
            PasswordHash = passwordHash,
            PermissionLevel = "UNSPECIFIED"
        });

        await _context.SaveChangesAsync();
        return LoginResult.Success;
    }
}