using Microsoft.EntityFrameworkCore;

namespace QuizAPI.Models;

public class Db : DbContext
{
	public Db(DbContextOptions<Db> options) : base(options)
	{

	}

	public DbSet<Quiz> Quizzes { get; init; } = null!;
	public DbSet<Question> Questions { get; init; } = null!;
	public DbSet<Answer> Answers { get; init; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
    }

}
