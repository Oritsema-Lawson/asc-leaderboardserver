using Microsoft.AspNetCore.Mvc;
using LeaderboardServer.DTO;
using LeaderboardServer.Data;
using LeaderboardServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeaderboardServer.Services;

public interface ILeaderboardService
{
    public ActionResult<List<MemberDTO>> GetTop(int num);
    public ActionResult<List<KataDTO>> GetKatas();
}

public class LeaderboardService : ILeaderboardService
{
    private readonly LeaderboardContext _context;

    public LeaderboardService(LeaderboardContext context)
    {
        _context = context;
    }

    public ActionResult<List<MemberDTO>> GetTop(int num = 10)
    {
        return _context.TrackedMember.OrderBy(x => x.TotalPoints)
                                .Take(num)
                                .Select(x => new MemberDTO(x.Username, x.TotalPoints))
                                .ToList();
    }

    public ActionResult<List<KataDTO>> GetKatas()
    {
        return _context.TrackedKata.OrderBy(x => -x.PointDifficulty)
                                .Select(x => new KataDTO(x.KataName, x.KataUrl))
                                .ToList();
    }

    public async Task LogCompletion(string memberUsername, string kataRemoteId)
    {
        var kata = await _context.TrackedKata.Where(x => x.KataRemoteId == kataRemoteId)
                                .FirstOrDefaultAsync();
        
        var member = await _context.TrackedMember.Where(x => x.Username == memberUsername)
                            .FirstOrDefaultAsync();

        if(kata is null || member is null)
            return;

        if(await _context.CompletionLog.AnyAsync(x => x.MemberId == member.Id && x.KataId == kata.Id))
            return;


        await _context.CompletionLog.AddAsync(new CompletionLogEntry
        {
            KataId = kata.Id,
            MemberId = member.Id,
            CompletedAt = DateTime.UtcNow
        });

        member.TotalPoints += kata.PointDifficulty;

        await _context.SaveChangesAsync();
    }

    public async Task<int> AddKata(string kataRemoteId, string kataName, string kataUrl, int pointDifficulty)
    {
        if(await _context.TrackedKata.AnyAsync(x => x.KataRemoteId == kataRemoteId))
            return 1; //Exists
        
        await _context.TrackedKata.AddAsync(new Kata
        {
           KataRemoteId = kataRemoteId,
           KataName = kataName,
           KataUrl = kataUrl,
           PointDifficulty = pointDifficulty 
        });

        return 0;
    }

}