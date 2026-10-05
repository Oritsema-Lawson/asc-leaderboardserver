public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    string CreatedBy { get; set; }
    DateTime? LastModifiedAt { get; set; }
    string? LastModifiedBy { get; set; }
    DateTime? DeletedAt { get; set; }
}

public abstract class AuditableEntity : IAuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = String.Empty;
    public DateTime? LastModifiedAt { get; set; } 
    public string? LastModifiedBy { get; set; }
    public DateTime? DeletedAt { get; set; } = null;
}