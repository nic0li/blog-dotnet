using Blog.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Blog.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUser(modelBuilder);
        ConfigureCategory(modelBuilder);
        ConfigurePost(modelBuilder);
        ConfigureComment(modelBuilder);
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(user => user.Email)
                .IsUnique();

            entity.Property(user => user.Email)
                .IsRequired();

            entity.Property(user => user.Password)
                .IsRequired();

            entity.Property(user => user.Role)
                .HasConversion<string>();
        });
    }

    private static void ConfigureCategory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasIndex(category => category.Name)
                .IsUnique();

            entity.Property(category => category.Name)
                .IsRequired();
        });
    }

    private static void ConfigurePost(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>(entity =>
        {
            entity.Property(post => post.Title)
                .IsRequired();

            entity.Property(post => post.Content)
                .IsRequired();

            entity.HasOne(post => post.User)
                .WithMany(user => user.Posts)
                .HasForeignKey(post => post.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(post => post.Category)
                .WithMany(category => category.Posts)
                .HasForeignKey(post => post.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureComment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.Property(comment => comment.Content)
                .IsRequired();

            entity.HasOne(comment => comment.User)
                .WithMany(user => user.Comments)
                .HasForeignKey(comment => comment.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(comment => comment.Post)
                .WithMany(post => post.Comments)
                .HasForeignKey(comment => comment.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public override async Task<int> SaveChangesAsync(
    CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

}