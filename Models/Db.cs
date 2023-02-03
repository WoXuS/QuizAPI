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

}
