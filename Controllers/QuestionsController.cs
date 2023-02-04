using Microsoft.AspNetCore.Mvc;
using QuizAPI.Models;

namespace QuizAPI.Controllers;

[Route("api/users/{userId}/quizzes/{quizId}/questions")]
[ApiController]
public class QuestionsController : ControllerBase
{
    private readonly Db db;

    public QuestionsController(Db context)
    {
        db = context;
    }

    // GET: api/users/1/quizzes/2/questions
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Question>>> GetQuestions(int userId, int quizId)
    {
        var quiz = await db.GetQuiz(userId, quizId);
        if (quiz is null)
        {
            return NotFound();
        }

        return quiz.Questions;
    }

    // GET: api/users/1/quizzes/2/questions/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Question>> GetQuestion(int userId, int quizId, int id)
    {
        var quiz = await db.GetQuiz(userId, quizId);

        return quiz?.Questions.FirstOrDefault(q => q.Id == id)
            is Question question
                ? Ok(question)
                : NotFound();
    }

    // PUT: api/users/1/quizzes/2/questions/3
    [HttpPut("{id}")]
    public async Task<IActionResult> PutQuestion(int userId, int quizId, int id, Question question)
    {
        if (id != question.Id)
        {
            return BadRequest();
        }

        var quiz = await db.GetQuiz(userId, quizId, true);
        if (quiz is null)
        {
            return NotFound();
        }

        var exists = quiz.Questions.Any(q => q.Id == id);
        if (!exists)
        {
            return NotFound();
        }

        db.Update(question);
        await db.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/users/1/quizzes/2/questions
    [HttpPost]
    public async Task<ActionResult<Question>> PostQuestion(int userId, int quizId, Question question)
    {
        var quiz = await db.GetQuiz(userId, quizId);
        if (quiz is null)
        {
            return NotFound();
        }

        quiz.Questions.Add(question);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetQuestion), new { userId, quizId, id = question.Id }, question);
    }

    // DELETE: api/users/1/quizzes/2/questions/3
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuestion(int userId, int quizId, int id)
    {
        var quiz = await db.GetQuiz(userId, quizId);
        if (quiz is null)
        {
            return NotFound();
        }

        if (quiz.Questions.FirstOrDefault(q => q.Id == id) is Question question)
        {
            quiz.Questions.Remove(question);
            await db.SaveChangesAsync();
            return NoContent();
        }

        return NotFound();
    }

}
