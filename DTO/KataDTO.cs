using LeaderboardServer.Entities;

namespace LeaderboardServer.DTO;

public class KataDTO
{
    public string KataName { get; set;}
    public string KataUrl { get; set; }

    public KataDTO(string name, string url)
    {
        KataName = name;
        KataUrl = url;
    }
}