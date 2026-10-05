using System.ComponentModel.DataAnnotations;
namespace LeaderboardServer.Entities;

public class CompletionLogEntry
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int MemberId { get; set; }

    [Required]
    public int KataId { get; set; }

    public DateTime CompletedAt { get; set;}
}