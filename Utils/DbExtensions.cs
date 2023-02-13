using Microsoft.EntityFrameworkCore;
using QuizAPI.Models;

namespace QuizAPI.Utils;

public static class DbExtensions
{
    public static async Task<Quiz?> GetQuizWithQuestions(this Db db, string userId, int quizId)
    {
        var quiz = await db.Quizzes
            .AsNoTracking()
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(q => q.Id == quizId && q.UserId == userId);
        
        return quiz;
    }

    public static async Task<(Quiz, Question)?> GetQuizAndQuestionWithAnswers(this Db db, string userId, int quizId, int questionId)
    {
        var quiz = await db.Quizzes
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == quizId && q.UserId == userId);

        if (quiz is null)
        {
            return null;
        }

        var question = await db.Questions
            .Include(q => q.Answers)
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == questionId);
        
        if (question is null)
        {
            return null;
        }

        return (quiz, question);
    }

    public static async Task LogAuthEvent(this Db db, string eventName, string userName, bool succeeded)
    {
        var authEvent = new AuthEvent()
        {
            Event = eventName,
            UserName = userName,
            Succeeded = succeeded,
            Time = DateTime.UtcNow
        };
        db.AuthEvents.Add(authEvent);
        await db.SaveChangesAsync();
    }
}
