using System.Drawing;
using APCServer.Entities;

namespace APCServer.DTO;

public class KataDTO
{
    public string KataRemoteId { get; set; } = string.Empty;
    public string KataName { get; set; } = string.Empty;
    public string KataUrl { get; set; } = string.Empty;
    public int PointDifficulty { get; set; }

    public KataDTO(string kataRemoteID, string kataName, string kataUrl, int pointDifficulty)
    {
        KataRemoteId = kataRemoteID;
        KataName = kataName;
        KataUrl = kataUrl;
        PointDifficulty = pointDifficulty;
    }
}