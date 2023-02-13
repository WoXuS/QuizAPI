using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizAPI.Attributes;
using QuizAPI.Models;
using QuizAPI.Utils;

namespace QuizAPI.Controllers;

[AuthorizeJwt]
[Route("api/quizzes")]
[ApiController]
public class QuizzesController : ControllerBase
{
    private readonly Db db;

    public QuizzesController(Db context)
    {
        db = context;
    }

    // GET: api/quizzes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Quiz>>> GetQuizzes()
    {
        var userId = User.GetUserID();
        var quizzes = db.Quizzes.Where(x => x.UserId == userId);

        return await quizzes.ToListAsync();
    }


    // GET: api/quizzes/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Quiz>> GetQuiz(int id)
    {
        var userId = User.GetUserID();
        var quiz = await db.Quizzes.FindAsync(id);

        return quiz is Quiz && quiz.UserId == userId
                ? quiz
                : NotFound();
    }

    // PUT: api/quizzes/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutQuiz(int id, Quiz quiz)
    {
        if (id != quiz.Id)
        {
            return BadRequest();
        }

        var userId = User.GetUserID();
        var existing = db.Quizzes.AsNoTracking().FirstOrDefault(q => q.Id == id && q.UserId == userId);
        if (existing is null)
        {
            return NotFound();
        }
        if (existing.IsOpen)
        {
            return Conflict();
        }

        quiz.IsOpen = false;
        quiz.UserId = userId!;
        db.Update(quiz);
        await db.SaveChangesAsync();

        return NoContent();
    }

    // PATCH: api/quizzes/5/open
    [HttpPatch("{id}/open")]
    public async Task<IActionResult> OpenQuiz(int id)
    {
        var userId = User.GetUserID();
        var existing = db.Quizzes.AsNoTracking().FirstOrDefault(q => q.Id == id && q.UserId == userId);
        if (existing is null)
        {
            return NotFound();
        }
        if (existing.IsOpen)
        {
            return BadRequest(new {Errors = new[] { "quiz is already open" } });
        }

        existing.IsOpen = true;
        db.Update(existing);
        await db.SaveChangesAsync();

        return NoContent();
    }

    // PATCH: api/quizzes/5/close
    [HttpPatch("{id}/close")]
    public async Task<IActionResult> CloseQuiz(int id)
    {
        var userId = User.GetUserID();
        var existing = db.Quizzes.AsNoTracking().FirstOrDefault(q => q.Id == id && q.UserId == userId);
        if (existing is null)
        {
            return NotFound();
        }
        if (!existing.IsOpen)
        {
            return BadRequest(new {Errors = new[] { "quiz is already closed" } });
        }

        existing.IsOpen = false;
        db.Update(existing);
        await db.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/quizzes
    [HttpPost]
    public async Task<ActionResult<Quiz>> PostQuiz(Quiz quiz)
    {
        var userId = User.GetUserID();
        quiz.IsOpen = false;
        quiz.UserId = userId!;
        db.Quizzes.Add(quiz);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetQuiz), new { id = quiz.Id }, quiz);
    }

    // DELETE: api/quizzes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuiz(int id)
    {
        var userId = User.GetUserID();
        
        if (await db.Quizzes.FirstOrDefaultAsync(q => q.Id == id && q.UserId == userId) is Quiz quiz)
        {
            if (quiz.IsOpen)
            {
                return Conflict();
            }

            db.Quizzes.Remove(quiz);
            await db.SaveChangesAsync();
            return NoContent();
        }

        return NotFound();
    }
}
