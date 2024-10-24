using Core.Entities;
using Infrastructure.Data.Database.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Database;

public class EventDbContext : IdentityDbContext
{
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<ParticipantEvent> ParticipantEvents => Set<ParticipantEvent>();

    public EventDbContext(DbContextOptions<EventDbContext> options) : base(options)
    {
        //Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new EventConfiguration());
        modelBuilder.ApplyConfiguration(new ParticipantConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());

        modelBuilder
            .Entity<User>()
            .HasOne(u => u.Participant)
            .WithOne(p => p.User)
            .HasForeignKey<Participant>(p => p.UserId);

        modelBuilder
            .Entity<Event>()
            .HasMany(e => e.Participants)
            .WithMany(p => p.Events)
            .UsingEntity<ParticipantEvent>(
            j => j
                .HasOne(ep => ep.Participant)
                .WithMany(p => p.ParticipantEvents)
                .HasForeignKey(ep => ep.ParticipantId),

            j => j
                .HasOne(ep => ep.Event)
                .WithMany(e => e.ParticipantEvents)
                .HasForeignKey(ep => ep.EventId),

            j =>
            {
                j.Property(ep => ep.RegistrationDateTime).HasDefaultValue(DateTime.MinValue);
                j.HasKey(ep => new { ep.EventId, ep.ParticipantId });
                j.ToTable("Registrations");
            });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }
}