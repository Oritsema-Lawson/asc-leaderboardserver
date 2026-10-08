using System.ComponentModel.DataAnnotations;
namespace APCServer.Entities;

public class Member : AuditableEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
}

