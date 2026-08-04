using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Models;

namespace TaskManagement.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<TaskAttachment> TaskAttachments { get; set; }
        public DbSet<TaskComment> TaskComments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    modelBuilder.Entity<User>()
    .HasIndex(x => x.Username)
    .IsUnique();
    modelBuilder.Entity<User>()
    .HasIndex(x => x.Email)
    .IsUnique();
    modelBuilder.Entity<User>()
    .Property(x => x.Role)
    .HasConversion<string>()
    .HasMaxLength(20);
    modelBuilder.Entity<User>().Property(x => x.Username).HasMaxLength(100);
    modelBuilder.Entity<User>().Property(x => x.Email).HasMaxLength(200);
    modelBuilder.Entity<User>().Property(x => x.PasswordHash).HasMaxLength(200);
    modelBuilder.Entity<User>().Property(x => x.FirstName).HasMaxLength(100);
    modelBuilder.Entity<User>().Property(x => x.LastName).HasMaxLength(100);
    modelBuilder.Entity<Category>().Property(x => x.Name).HasMaxLength(100);
    modelBuilder.Entity<Category>().Property(x => x.Description).HasMaxLength(500);
    modelBuilder.Entity<Category>().Property(x => x.Color).HasMaxLength(20);
    modelBuilder.Entity<TaskItem>().Property(x => x.Title).HasMaxLength(200);
    modelBuilder.Entity<TaskItem>().Property(x => x.Description).HasMaxLength(2000);
    modelBuilder.Entity<TaskAttachment>().Property(x => x.FileName).HasMaxLength(255);
    modelBuilder.Entity<TaskAttachment>().Property(x => x.FilePath).HasMaxLength(1000);
    modelBuilder.Entity<TaskAttachment>().Property(x => x.ContentType).HasMaxLength(100);
    modelBuilder.Entity<TaskComment>().Property(x => x.Comment).HasMaxLength(2000);
    modelBuilder.Entity<Category>()
    .HasOne(c => c.User)
    .WithMany(u => u.Categories)
    .HasForeignKey(c => c.UserId)
    .OnDelete(DeleteBehavior.Cascade);
    modelBuilder.Entity<TaskItem>()
    .HasOne(t => t.User)
    .WithMany(u => u.Tasks)
    .HasForeignKey(t => t.UserId)
    .OnDelete(DeleteBehavior.Cascade);
    modelBuilder.Entity<TaskItem>()
    .HasOne(t => t.Category)
    .WithMany(c => c.Tasks)
    .HasForeignKey(t => t.CategoryId)
    .OnDelete(DeleteBehavior.SetNull);

    modelBuilder.Entity<TaskAttachment>()
    .HasOne(ta => ta.Task)
    .WithMany(t => t.Attachments)
    .HasForeignKey(ta => ta.TaskId)
    .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<TaskComment>()
    .HasOne(tc => tc.Task)
    .WithMany(t => t.Comments)
    .HasForeignKey(tc => tc.TaskId)
    .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<TaskComment>()
    .HasOne(tc => tc.User)
    .WithMany(u => u.TaskComments)
    .HasForeignKey(tc => tc.UserId)
    .OnDelete(DeleteBehavior.Cascade);
    modelBuilder.Entity<User>().HasData(
    new User
    {
        Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
        Username = "demo",
        Email = "demo@example.com",
        PasswordHash = "$2a$12$cGT9TW5Yj0qase79ysDFju.BCTU3/xUjXlUSrPTyL0RHupdWLcqku",
        FirstName = "Demo",
        LastName = "User",
        CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        IsActive = true,
        Role = UserRole.User
    }
);
}
    }
}
