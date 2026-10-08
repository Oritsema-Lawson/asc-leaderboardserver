using System.ComponentModel.DataAnnotations;

namespace APCServer.Entities;

public class User : AuditableEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserEmail { get; set; } = String.Empty;

    [Required]
    public string PasswordHash { get; set; } = String.Empty;
    
    [Required]
    [AllowedValues("ADMIN", "USER", "UNSPECIFIED", ErrorMessage = "Unknown Value")]
    public string PermissionLevel { get; set; } = String.Empty;
}