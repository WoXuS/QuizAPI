using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizAPI.Attributes;
using QuizAPI.Models;
using QuizAPI.Utils;

namespace QuizAPI.Controllers;

[AuthorizeJwt]
[Route("api/attempts")]
[ApiController]
public class AttemptsController : ControllerBase
{
    private readonly Db db;

    public AttemptsController(Db context)
    {
        db = context;
    }

    // GET: api/attempts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Attempt>>> GetAttempts()
    {
        var userId = User.GetUserID();
        var attempts = db.Attempts.Where(x => x.UserId == userId);

        return await attempts.ToListAsync();
    }

    // GET: api/attempts/2
    [HttpGet("{id}")]
    public async Task<ActionResult<Attempt>> GetAttempt(int id)
    {
        var userId = User.GetUserID();

        return await db.Attempts.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId)
            is Attempt attempt
                ? attempt
                : NotFound();
    }

    public record AttemptUpdateDto(int[] ChosenAnswers);

    // PATCH: api/attempts/2
    [HttpPatch("{id}")]
    public async Task<IActionResult> PutAttempt(int id, AttemptUpdateDto dto)
    {
        var userId = User.GetUserID();

        var existing = await db.Attempts.FirstOrDefaultAsync(a => a.Id == id);
        if (existing is null)
        {
            return NotFound();
        }
        if (!existing.IsOpen)
        {
            return Conflict();
        }

        var updateIsValid = IsUpdateValid(dto, existing);
        if (!updateIsValid)
        {
            return NotFound();
        }

        existing.ChosenAnswers = dto.ChosenAnswers;
        await db.SaveChangesAsync();

        return NoContent();


        bool IsUpdateValid(AttemptUpdateDto dto, Attempt existing)
        {
            var newAnswers = dto.ChosenAnswers
                .Except(existing.ChosenAnswers)
                .ToArray();

            var validAnswerIds = existing.QuizCopy.Questions
                .SelectMany(q => q.Answers)
                .Select(a => a.Id)
                .ToArray();

            return newAnswers.All(a => validAnswerIds.Contains(a));
        }
    }

    // PATCH: api/attempts/2/close
    [HttpPatch("{id}/close")]
    public async Task<IActionResult> CloseAttempt(int id)
    {
        var existing = await db.Attempts.FirstOrDefaultAsync(a => a.Id == id);
        if (existing is null)
        {
            return NotFound();
        }
        if (!existing.IsOpen)
        {
            return Conflict();
        }

        existing.IsOpen = false;
        existing.Result = CreateResult(existing);
        await db.SaveChangesAsync();

        return NoContent();
    }

    private Result CreateResult(Attempt existing)
    {
        var answers = existing.QuizCopy.Questions
                .SelectMany(q => q.Answers)
                .Select(a => new { a.Id, a.IsCorrect })
                .GroupBy(a => a.IsCorrect);

        var corrects = answers.Single(g => g.Key is true)
            .Select(a => a.Id)
            .ToList();

        var incorrects = answers.Single(g => g.Key is false)
            .Select(a => a.Id)
            .ToList();

        var chosen = existing.ChosenAnswers;

        var result = new Result
        {
            Submitted = DateTime.UtcNow,
            AllCorrectAnswers = corrects.Count,
            ChosenCorrectAnswers = chosen.Intersect(corrects).Count(),
            ChosenIncorrectAnswers = chosen.Intersect(incorrects).Count()
        };

        return result;
    }

    // GET: api/attempts/{id}/result
    [HttpGet("{id}/result")]
    public async Task<ActionResult<Result>> GetResult(int id)
    {
        var userId = User.GetUserID();
        return (await db.Attempts.Include(a => a.Result)
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId))
            ?.Result is Result result
                ? result
                : NotFound();
    }

    // POST: api/attempts
    [HttpPost]
    public async Task<ActionResult<Attempt>> PostAttempt(int quizId)
    {
        var userId = User.GetUserID();

        var quiz = await db.Quizzes
            .Include(q => q.Questions)
            .ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == quizId);

        if (quiz is null)
        {
            return NotFound();
        }

        if (!quiz.IsOpen)
        {
            return Conflict();
        }

        var questionOrder = GetQuesionOrder(quiz.Questions);

        var attempt = new Attempt
        {
            QuizId = quizId,
            IsOpen = true,
            QuestionOrder = questionOrder,
            UserId = userId!,
            QuizCopy = new() {
                Id = quiz.Id,
                Description = quiz.Description,
                Name= quiz.Name,
                Questions = CopyQuestionsInOrder(quiz.Questions, questionOrder),
                UserId = quiz.UserId
            } 
        };

        db.Attempts.Add(attempt);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAttempt), new { id = attempt.Id }, attempt);



        static List<QuestionCopy> CopyQuestionsInOrder(List<Question> questions, int[] questionOrder) 
            => questionOrder.Select(o => questions.First(q => q.Id == o))
               .Select(q => new QuestionCopy { Id = q.Id, Answers = q.Answers, Text = q.Text })
               .ToList();

        static int[] GetQuesionOrder(IEnumerable<Question> questions)
        {
            var ids = questions.Select(q => q.Id).ToList();

            var rnd = new Random();
            return ids.OrderBy(x => rnd.Next()).ToArray();
        }
    }

    // DELETE: api/attempts/2
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAttempt(int id)
    {
        var userId = User.GetUserID();

        if (await db.Attempts.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId) is Attempt attempt)
        {
            if (!attempt.IsOpen)
            {
                return Conflict();
            }

            db.Attempts.Remove(attempt);
            await db.SaveChangesAsync();
            return NoContent();
        }

        return NotFound();
    }

}
