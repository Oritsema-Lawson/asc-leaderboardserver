using System.ComponentModel.DataAnnotations;
namespace LeaderboardServer.Entities;

public class Kata
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(64)]
    public string KataRemoteId { get; set; } = string.Empty;
    public string KataName { get; set; } = string.Empty;
    public string KataUrl { get; set; } = string.Empty; // String.empty just to sate the non nullable thingy
    public int PointDifficulty { get; set; }
}

