using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizAPI.Models;

namespace QuizAPI.Controllers;

[Route("api/users/{userId}/attempts")]
[ApiController]
public class AttemptsController : ControllerBase
{
    private readonly Db db;

    public AttemptsController(Db context)
    {
        db = context;
    }

    // GET: api/users/1/attempts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Attempt>>> GetAttempts(int userId)
    {
        var user = await db.GetUserWithAttempts(userId);

        if (user is null)
        {
            return NotFound();
        }

        return user.Attempts;
    }

    // GET: api/users/1/attempts/2
    [HttpGet("{id}")]
    public async Task<ActionResult<Attempt>> GetAttempt(int userId, int id)
    {
        var user = await db.GetUserWithAttempts(userId);

        return user?.Attempts.FirstOrDefault(a => a.Id == id)
            is Attempt attempt
                ? attempt
                : NotFound();
    }

    // PUT: api/users/1/attempts/2
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAttempt(int userId, int id, Attempt attempt)
    {
        if (id != attempt.Id)
        {
            return BadRequest();
        }

        var user = await db.GetUserWithAttempts(userId);
        if (user is null)
        {
            return NotFound();
        }

        var quiz = await db.Quizzes
            .Include(q => q.Questions)
            .ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == attempt.QuizId);

        if (quiz is null)
        {
            return NotFound();
        }

        var existing = user.Attempts.FirstOrDefault(a => a.Id == id);
        if (existing is null)
        {
            return NotFound();
        }

        var updateIsValid = IsUpdateValid(attempt, existing, quiz);
        if (!updateIsValid) 
        {
            return NotFound();
        }

        existing.ChosenAnswers = attempt.ChosenAnswers;
        await db.SaveChangesAsync();

        return NoContent();



        bool IsUpdateValid(Attempt attempt, Attempt existing, Quiz quiz)
        {
            var newAnswers = attempt.ChosenAnswers
                .Except(existing.ChosenAnswers)
                .ToArray();

            var validAnswerIds = quiz.Questions
                .SelectMany(q => q.Answers)
                .Select(a => a.Id)
                .ToArray();

            var allNewAreValid = newAnswers.All(a => validAnswerIds.Contains(a));
            return allNewAreValid;
        }
    }

    // POST: api/users/1/attempts
    [HttpPost]
    public async Task<ActionResult<Attempt>> PostAttempt(int userId, Attempt attempt)
    {
        var user = await db.GetUserWithAttempts(userId);
        if (user is null)
        {
            return NotFound();
        }

        var quiz = await db.Quizzes
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(q => q.Id == attempt.QuizId);

        if (quiz is null)
        {
            return NotFound();
        }

        if (!quiz.IsOpen)
        {
            return Conflict();
        }

        SetQuesionOrder(attempt, quiz);
        attempt.ChosenAnswers = Array.Empty<int>();
        user.Attempts.Add(attempt);

        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAttempt), new { userId, id = attempt.Id }, attempt);


        static void SetQuesionOrder(Attempt attempt, Quiz quiz)
        {
            var ids = quiz!.Questions.Select(q => q.Id).ToList();

            var rnd = new Random();
            attempt.QuestionOrder = ids.OrderBy(x => rnd.Next()).ToArray();
        }
    }

    // DELETE: api/users/1/attempts/2
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAttempt(int userId, int id)
    {
        var user = await db.GetUserWithAttempts(userId);
        if (user is null)
        {
            return NotFound();
        }

        if (user.Attempts.FirstOrDefault(a => a.Id == id) is Attempt attempt)
        {
            user.Attempts.Remove(attempt);
            await db.SaveChangesAsync();
            return NoContent();
        }

        return NotFound();
    }

}
