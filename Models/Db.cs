using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace QuizAPI.Models;

public class Db : DbContext
{
    public Db(DbContextOptions<Db> options) : base(options)
    {

    }

    public DbSet<User> Users { get; init; } = null!;
    public DbSet<Quiz> Quizzes { get; init; } = null!;
    public DbSet<Question> Questions { get; init; } = null!;
    public DbSet<Answer> Answers { get; init; } = null!;
    public DbSet<Attempt> Attempts { get; init; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureUser(modelBuilder);

        modelBuilder.Entity<Quiz>()
            .HasMany(q => q.Questions)
            .WithOne()
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Question>()
            .HasMany(q => q.Answers)
            .WithOne()
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        ConfigureAttempt(modelBuilder);

        static void ConfigureUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Quizzes)
                .WithOne()
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasMany(u => u.Attempts)
                .WithOne()
                .IsRequired();
        }

        static void ConfigureAttempt(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attempt>()
                .HasOne<Quiz>()
                .WithMany()
                .HasForeignKey(a => a.QuizId)
                .IsRequired();

            modelBuilder.Entity<Attempt>()
                .Property(a => a.QuestionOrder)
                .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<int[]>(v, (JsonSerializerOptions?)null) ?? Array.Empty<int>()
                );

            modelBuilder.Entity<Attempt>()
                .Property(a => a.ChosenAnswers)
                .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<int[]>(v, (JsonSerializerOptions?)null) ?? Array.Empty<int>()
                );
        }
    }

}
