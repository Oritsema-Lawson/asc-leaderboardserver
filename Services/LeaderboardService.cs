using Microsoft.AspNetCore.Mvc;
using APCServer.DTO;
using APCServer.Data;
using APCServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace APCServer.Services;

public interface ILeaderboardService
{
    public Task<List<MemberDTO>> GetTop(int num);
    public Task<List<MemberDTO>> GetAllUsers();
    public Task<List<KataDTO>> GetKatas();
    public Task<int> LogCompletion(string memberUsername, string kataRemoteId);
    public Task<int> AddKata(string kataRemoteId, string kataName, string kataUrl, int pointDifficulty);
    public Task<int> DelKata(string kataRemoteId);
    public Task<int> AddMember(string username);
    public Task<int> DelMember(string username);
}

public class LeaderboardService : ILeaderboardService
{
    private readonly ApcDbContext _context;

    public LeaderboardService(ApcDbContext context)
    {
        _context = context;
    }

    public async Task<List<MemberDTO>> GetTop(int num = 10)
    {
        return await _context.TrackedMember.OrderByDescending(x => x.TotalPoints)
                                .Take(num)
                                .Select(x => new MemberDTO(x.Username, x.TotalPoints))
                                .ToListAsync();
    }

    public async Task<List<MemberDTO>> GetAllUsers()
    {
        return await _context.TrackedMember.OrderByDescending(x => x.TotalPoints)
                                .Select(x => new MemberDTO(x.Username, x.TotalPoints))
                                .ToListAsync();
    }


    public async Task<List<KataDTO>> GetKatas()
    {
        return await _context.TrackedKata.OrderByDescending(x => x.PointDifficulty)
                                .Select(x => new KataDTO(x.KataRemoteId, x.KataName, x.KataUrl, x.PointDifficulty))
                                .ToListAsync();
    }

    public async Task<int> LogCompletion(string memberUsername, string kataRemoteId)
    {
        var kata = await _context.TrackedKata.Where(x => x.KataRemoteId == kataRemoteId)
                                .FirstOrDefaultAsync();
        
        var member = await _context.TrackedMember.Where(x => x.Username == memberUsername)
                            .FirstOrDefaultAsync();

        if(kata is null || member is null)
            return 1;

        if(await _context.CompletionLog.AnyAsync(x => x.MemberId == member.Id && x.KataId == kata.Id))
            return 2;


        _context.CompletionLog.Add(new CompletionLogEntry
        {
            KataId = kata.Id,
            MemberId = member.Id,
            CompletedAt = DateTime.UtcNow
        });

        member.TotalPoints += kata.PointDifficulty;

        await _context.SaveChangesAsync();
        return 0;
    }

    public async Task<int> AddKata(string kataRemoteId, string kataName, string kataUrl, int pointDifficulty)
    {
        if(await _context.TrackedKata.AnyAsync(x => x.KataRemoteId == kataRemoteId))
            return 1; //Exists
        
        _context.TrackedKata.Add(new Kata
        {
           KataRemoteId = kataRemoteId,
           KataName = kataName,
           KataUrl = kataUrl,
           PointDifficulty = pointDifficulty 
        });

        await _context.SaveChangesAsync();

        return 0;
    }

    public async Task<int> DelKata(string kataRemoteId)
    {

        var kata = await _context.TrackedKata.FirstOrDefaultAsync(x => x.KataRemoteId == kataRemoteId);
        if(kata is null)
            return 1; //Does not exist

        var affectedLogs = await _context.CompletionLog.Where(x => kata.Id == x.KataId).ToListAsync();
        var affectedMemberIds = affectedLogs.Select(x => x.MemberId).Distinct().ToList();
        var affectedMembers = await _context.TrackedMember.Where(x => affectedMemberIds.Contains(x.Id)).ToListAsync();

        foreach(Member member in affectedMembers)
        {
            member.TotalPoints -= kata.PointDifficulty;
        }
        
        _context.TrackedKata.Remove(kata); //SHOULD soft delete
        _context.CompletionLog.RemoveRange(affectedLogs);

        await _context.SaveChangesAsync();

        return 0;
    }

    public async Task<int> AddMember(string username)
    {
        if(await _context.TrackedMember.AnyAsync(x => x.Username == username))
            return 1; //Exists
        
        _context.TrackedMember.Add(new Member
        {
           Username = username,
           TotalPoints = 0
        });

        await _context.SaveChangesAsync();

        return 0;
    }

    public async Task<int> DelMember(string username)
    {

        var member = await _context.TrackedMember.FirstOrDefaultAsync(x => x.Username == username);
        if(member is null)
            return 1; 

        _context.TrackedMember.Remove(member); //SHOULD soft delete

        await _context.SaveChangesAsync();

        return 0;
    }
}