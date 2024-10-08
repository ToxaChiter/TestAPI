using Microsoft.EntityFrameworkCore;
using TestAPI.Database.Configuration;
using TestAPI.Models;

namespace TestAPI.Database;

public class EventDbContext : DbContext
{ 
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<User> Users => Set<User>();
    public DbSet<ParticipantEvent> ParticipantEvents => Set<ParticipantEvent>();

    public EventDbContext(DbContextOptions<EventDbContext> options) : base(options)
    {
        Database.EnsureDeleted();
        Database.EnsureCreated();

        Users.Update(new User() { Login = "Admin", Password = "Admin" });
        SaveChanges();

        Participants.Update(new Participant() { FirstName = "Anton", UserId = 1, Email = "admin@gmail.com" });
        SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
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