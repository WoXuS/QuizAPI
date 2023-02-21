using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using QuizAPI.Utils;

namespace QuizAPI.Models;

public class Db : IdentityDbContext<User>
{
    public Db(DbContextOptions<Db> options) : base(options)
    {
    }

    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<AuthEvent> AuthEvents { get; set; } = null!;

    public DbSet<User> AppUsers { get; init; } = null!;
    public DbSet<Quiz> Quizzes { get; init; } = null!;
    public DbSet<Question> Questions { get; init; } = null!;
    public DbSet<Answer> Answers { get; init; } = null!;
    public DbSet<Attempt> Attempts { get; init; } = null!;
    public DbSet<Result> Results { get; init; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureIdentityEntities(builder);
        ConfigureUser(builder);

        builder.Entity<Quiz>()
            .HasMany(q => q.Questions)
            .WithOne()
            .IsRequired()
            .HasForeignKey(q => q.QuizId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Question>()
            .HasMany(q => q.Answers)
            .WithOne()
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        ConfigureAttempt(builder);
    }

    private static void ConfigureIdentityEntities(ModelBuilder builder)
    {
        builder.Entity<RefreshToken>()
                .HasKey(rt => rt.Token);

        builder.Entity<RefreshToken>()
                .HasOne<User>()
                .WithMany()
                .IsRequired()
                .HasForeignKey(rt => rt.UserId);

        var adminId = "b5818efb-f28f-4486-a784-9f1eb44f51e1";
        var userRoleId = "cfd8ddf0-7268-4c54-a98e-9defed5f65b7";
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = adminId,
                Name = "Admin",
                NormalizedName = "ADMIN".ToUpper()
            },
            new IdentityRole
            {
                Id = userRoleId,
                Name = "User",
                NormalizedName = "USER".ToUpper()
            }
        );

        var hasher = new PasswordHasher<User>();
        builder.Entity<User>().HasData(new User
        {
            Id = adminId,
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@admin.admin",
            NormalizedEmail = "admin@admin.admin".ToUpper(),
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null!, "admin"),
            SecurityStamp = string.Empty
        });

        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                RoleId = adminId,
                UserId = adminId
            },
            new IdentityUserRole<string>
            {
                RoleId = userRoleId,
                UserId = adminId
            }
        );
    }

    private static void ConfigureUser(ModelBuilder builder)
    {
        builder.Entity<User>()
            .HasMany(u => u.Quizzes)
            .WithOne()
            .HasForeignKey(q => q.UserId)
            .IsRequired();

        builder.Entity<User>()
            .HasMany(u => u.Attempts)
            .WithOne()
            .HasForeignKey(a => a.UserId)
            .IsRequired();
    }

    private static void ConfigureAttempt(ModelBuilder builder)
    {
        builder.Entity<Attempt>()
            .HasOne<Quiz>()
            .WithMany()
            .HasForeignKey(a => a.QuizId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Attempt>()
            .Property(a => a.QuestionOrder)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<int[]>(v, (JsonSerializerOptions?)null) ?? Array.Empty<int>(),
                new ValueComparer<int[]>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToArray()
                )
            );

        builder.Entity<Attempt>()
            .Property(a => a.ChosenAnswers)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<int[]>(v, (JsonSerializerOptions?)null) ?? Array.Empty<int>(),
                new ValueComparer<int[]>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToArray()
                )
            );

        builder.Entity<Attempt>()
            .Property(a => a.QuizCopy)
            .HasColumnType("nvarchar(max)")
            .HasConversion(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<QuizCopy>(v, (JsonSerializerOptions?)null)!,
            new ValueComparer<QuizCopy>(
            (l, r) => JsonSerializer.Serialize(l, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(r, (JsonSerializerOptions?)null)!,
            v => v == null ? 0 : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null).GetHashCode(),
            v => JsonSerializer.Deserialize<QuizCopy>(
                JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            (JsonSerializerOptions?)null)!
            )
        );

    }
}
