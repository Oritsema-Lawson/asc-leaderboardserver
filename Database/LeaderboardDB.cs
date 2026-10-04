using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace LeaderboardServer;

public class LeaderboardContext : DbContext
{
    public DbSet<Kata> TrackedKata => Set<Kata>();
    public DbSet<Member> TrackedMember => Set<Member>();
    public DbSet<CompletionLogEntry> CompletionLog => Set<CompletionLogEntry>();

    public string DbPath { get; }

    public LeaderboardContext(DbContextOptions<LeaderboardContext> options) : base(options) {}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder) 
        => modelBuilder.Entity<CompletionLogEntry>().HasIndex(x => new {x.MemberId, x.KataId}).IsUnique();
}

public class Kata
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(64)]
    public string KataRemoteId { get; set; }
    public string KataUrl { get; set; }
    public int PointDIfficulty { get; set; }
}

public class Member
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Username {get; set; } = string.Empty;
    public int TotalPoints { get; set; }
}

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