using APCServer.Clients;
using APCServer.Entities;
using APCServer.DTO;
using APCServer.Services;

namespace APCServer.Workers;

public class CodewarsPollingWorker : BackgroundService
{
    private readonly CodewarsClient _client;
    private readonly IServiceScopeFactory _factory;

    public CodewarsPollingWorker(CodewarsClient client, IServiceScopeFactory factory)
    {
        _client = client;
        _factory = factory;
    }

    protected override async Task ExecuteAsync(CancellationToken cancel)
    {
        PeriodicTimer fullCooldown = new PeriodicTimer(TimeSpan.FromSeconds(10));

        while(!cancel.IsCancellationRequested)
        {
            await fullCooldown.WaitForNextTickAsync(cancel);

            using(var scope = _factory.CreateScope())
            {
                var leaderboardService = scope.ServiceProvider.GetRequiredService<ILeaderboardService>();
                List<KataDTO> kataList = await leaderboardService.GetKatas();

                foreach(MemberDTO m in await leaderboardService.GetAllUsers())
                {
                    List<string?> completedKataIds = await _client.GetCompletedKata(m.Username);

                    List<KataDTO> validCompletedKata = kataList.Where(x => completedKataIds.Contains(x.KataRemoteId))
                                                                .ToList();

                    foreach(KataDTO k in validCompletedKata) 
                        await leaderboardService.LogCompletion(m.Username, k.KataRemoteId);
                    
                    await Task.Delay(TimeSpan.FromSeconds(15), cancel); 
                }
            }
        }
    }
}