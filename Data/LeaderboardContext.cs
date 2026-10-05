using Microsoft.EntityFrameworkCore;
using LeaderboardServer.Entities;

namespace LeaderboardServer.Data;

public class LeaderboardContext : DbContext
{
    public DbSet<Kata> TrackedKata => Set<Kata>();
    public DbSet<Member> TrackedMember => Set<Member>();
    public DbSet<CompletionLogEntry> CompletionLog => Set<CompletionLogEntry>();

    public LeaderboardContext(DbContextOptions<LeaderboardContext> options) : base(options) {}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder) 
        { 
            modelBuilder.Entity<CompletionLogEntry>().HasIndex(x => new {x.MemberId, x.KataId}).IsUnique();

            modelBuilder.Entity<Kata>().HasQueryFilter(x => x.DeletedAt == null);
            modelBuilder.Entity<Member>().HasQueryFilter(x => x.DeletedAt == null);
            modelBuilder.Entity<CompletionLogEntry>().HasQueryFilter(x => x.DeletedAt == null);
        }

    public override async Task<int> SaveChangesAsync(CancellationToken token = default)
    {
        var auditable = ChangeTracker.Entries<IAuditableEntity>();

        foreach(var entry in auditable)
        {
            DateTime currentTime = DateTime.UtcNow;
            string user = "System"; //Change to actual user later

            switch(entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = currentTime;
                    entry.Entity.CreatedBy = user;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedAt = currentTime;
                    entry.Entity.LastModifiedBy = user;
                    break;

                case EntityState.Deleted: //Soft deletion
                    entry.State = EntityState.Modified;
                    entry.Entity.DeletedAt = currentTime;
                    entry.Entity.LastModifiedBy = user;
                    break;
            }
        }

        return await base.SaveChangesAsync(token);
    }
}

