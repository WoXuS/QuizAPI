using Microsoft.AspNetCore.Mvc;
using QuizAPI.Attributes;
using QuizAPI.Models;
using QuizAPI.Utils;

namespace QuizAPI.Controllers;

[AuthorizeJwt]
[Route("api/quizzes/{quizId}/questions")]
[ApiController]
public class QuestionsController : ControllerBase
{
    private readonly Db db;

    public QuestionsController(Db context)
    {
        db = context;
    }

    // GET: api/quizzes/2/questions
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Question>>> GetQuestions(int quizId)
    {
        var userId = User.GetUserID();
        var quiz = await db.GetQuizWithQuestions(userId!, quizId);
        if (quiz is null)
        {
            return NotFound();
        }

        return quiz.Questions;
    }

    // GET: api/quizzes/2/questions/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Question>> GetQuestion(int quizId, int id)
    {
        var userId = User.GetUserID();
        var quiz = await db.GetQuizWithQuestions(userId!, quizId);

        return quiz?.Questions.FirstOrDefault(q => q.Id == id)
            is Question question
                ? question
                : NotFound();
    }

    // PUT: api/quizzes/2/questions/3
    [HttpPut("{id}")]
    public async Task<IActionResult> PutQuestion(int quizId, int id, Question question)
    {
        if (id != question.Id)
        {
            return BadRequest();
        }

        var userId = User.GetUserID();
        var quiz = await db.GetQuizWithQuestions(userId!, quizId);

        if (quiz is null)
        {
            return NotFound();
        }
        if (quiz.IsOpen)
        {
            return Conflict();
        }

        var exists = quiz.Questions.Any(q => q.Id == id);
        if (!exists)
        {
            return NotFound();
        }

        question.QuizId = quizId;
        db.Update(question);
        await db.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/quizzes/2/questions
    [HttpPost]
    public async Task<ActionResult<Question>> PostQuestion(int quizId, Question question)
    {
        var userId = User.GetUserID();
        var quiz = await db.GetQuizWithQuestions(userId!, quizId);

        if (quiz is null)
        {
            return NotFound();
        }
        if (quiz.IsOpen)
        {
            return Conflict();
        }

        question.QuizId = quizId;
        db.Questions.Add(question);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetQuestion), new { quizId, id = question.Id }, question);
    }

    // DELETE: api/quizzes/2/questions/3
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuestion(int quizId, int id)
    {
        var userId = User.GetUserID();
        var quiz = await db.GetQuizWithQuestions(userId!, quizId);

        if (quiz is null)
        {
            return NotFound();
        }
        if (quiz.IsOpen)
        {
            return Conflict();
        }

        if (quiz.Questions.FirstOrDefault(q => q.Id == id) is Question question)
        {
            db.Questions.Remove(question);
            await db.SaveChangesAsync();
            return NoContent();
        }

        return NotFound();
    }

}
