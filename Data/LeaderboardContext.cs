using Microsoft.EntityFrameworkCore;
using LeaderboardServer.Entities;

namespace LeaderboardServer.Data;

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

