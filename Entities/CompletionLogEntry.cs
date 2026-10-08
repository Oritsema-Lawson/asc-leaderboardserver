using System.ComponentModel.DataAnnotations;
namespace APCServer.Entities;

public class CompletionLogEntry : AuditableEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int MemberId { get; set; }

    [Required]
    public int KataId { get; set; }

    public DateTime CompletedAt { get; set;}
}