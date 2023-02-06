using Microsoft.EntityFrameworkCore;
using QuizAPI.Models;

namespace QuizAPI.Controllers;

public static class DbExtensions
{
    public static async Task<User?> GetUser(this Db db, int userId, bool AsNoTracking = false)
    {
        var query = db.Users
           .Include(u => u.Quizzes)
           .ThenInclude(q => q.Questions)
           .ThenInclude(q => q.Answers);

        var user = await (AsNoTracking
            ? query.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId)
            : query.FirstOrDefaultAsync(u => u.Id == userId)
            );
        return user;
    }

    public static async Task<Quiz?> GetQuiz(this Db db, int userId, int quizId, bool AsNoTracking = false)
    {
        var query = db.Users
           .Include(u => u.Quizzes)
           .ThenInclude(q => q.Questions)
           .ThenInclude(q => q.Answers);

        var user = await (AsNoTracking
            ? query.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId)
            : query.FirstOrDefaultAsync(u => u.Id == userId)
            );
        if (user is null)
        {
            return null;
        }

        var quiz = user.Quizzes.FirstOrDefault(quiz => quiz.Id == quizId);
        return quiz;
    }

    public static async Task<Question?> GetQuestion(this Db db, int userId, int quizId, int questionId, bool AsNoTracking = false)
    {
        var query = db.Users
           .Include(u => u.Quizzes)
           .ThenInclude(q => q.Questions)
           .ThenInclude(q => q.Answers);

        var user = await (AsNoTracking
            ? query.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId)
            : query.FirstOrDefaultAsync(u => u.Id == userId)
            );
        if (user is null)
        {
            return null;
        }

        var quiz = user.Quizzes.FirstOrDefault(quiz => quiz.Id == quizId);
        if (quiz is null)
        {
            return null;
        }

        var question = quiz.Questions.FirstOrDefault(q => q.Id == questionId);
        return question;
    }
    
    public static async Task<User?> GetUserWithAttempts(this Db db, int userId)
    {
        var user = await db.Users
            .Include(x => x.Attempts)
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user;
    }
}
