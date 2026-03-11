using Microsoft.EntityFrameworkCore;
using NotificationsAPI.Domain.Entities;

namespace NotificationsAPI.Infrastructure.Persistence;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Notification>(entity =>
        {

            entity.ToTable("Notifications");

            entity.HasKey(n => n.Id);
            
            entity.Property(n => n.Id)
                  .IsRequired();

            entity.Property(n => n.UserEmail)
                  .HasColumnType("VARCHAR(512)").IsRequired();
            

            entity.Property(n => n.Subject)
                  .HasColumnType("VARCHAR(512)");


            entity.Property(n => n.Body)
                  .HasColumnType("VARCHAR(MAX)");

            entity.Property(n => n.Type)
                  .HasColumnType("INT").IsRequired();

            entity.Property(n => n.ErrorMessage)
                   .HasColumnType("VARCHAR(1024)");
     
            entity.HasIndex(n => n.UserId);

            entity.Property(n => n.CreatedAt)
                  .HasColumnType("DATETIME");

            entity.Property(n => n.SentAt)
                  .HasColumnType("DATETIME");

    });
    }
}