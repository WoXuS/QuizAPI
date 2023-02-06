using Microsoft.AspNetCore.Mvc;
using QuizAPI.Models;

namespace QuizAPI.Controllers;

[Route("api/users/{userId}/quizzes")]
[ApiController]
public class QuizzesController : ControllerBase
{
    private readonly Db db;

    public QuizzesController(Db context)
    {
        db = context;
    }

    // GET: api/users/1/quizzes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Quiz>>> GetQuizzes(int userId)
    {
        var user = await db.GetUser(userId);
        if (user is null)
        {
            return NotFound();
        }

        return user.Quizzes;
    }


    // GET: api/users/1/quizzes/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Quiz>> GetQuiz(int userId, int id)
    {
        var user = await db.GetUser(userId);

        return user?.Quizzes.FirstOrDefault(q => q.Id == id)
            is Quiz quiz
                ? quiz
                : NotFound();
    }

    // PUT: api/users/1/quizzes/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutQuiz(int userId, int id, Quiz quiz)
    {
        if (id != quiz.Id)
        {
            return BadRequest();
        }

        var user = await db.GetUser(userId, true);
        if (user is null)
        {
            return NotFound();
        }

        var exists = user.Quizzes.Any(q => q.Id == id);
        if (!exists)
        {
            return NotFound();
        }

        db.Update(quiz);
        await db.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/users/1/quizzes
    [HttpPost]
    public async Task<ActionResult<Quiz>> PostQuiz(int userId, Quiz quiz)
    {
        var user = await db.GetUser(userId);
        if (user is null)
        {
            return NotFound();
        }

        user.Quizzes.Add(quiz);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetQuiz), new { userId, id = quiz.Id }, quiz);
    }

    // DELETE: api/users/1/quizzes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuiz(int userId, int id)
    {
        var user = await db.GetUser(userId);
        if (user is null)
        {
            return NotFound();
        }

        if (user.Quizzes.FirstOrDefault(q => q.Id == id) is Quiz quiz)
        {
            user.Quizzes.Remove(quiz);
            await db.SaveChangesAsync();
            return NoContent();
        }

        return NotFound();
    }
}
