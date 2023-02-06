using Microsoft.AspNetCore.Mvc;
using QuizAPI.Models;

namespace QuizAPI.Controllers;

[Route("api/users/{userId}/quizzes/{quizId}/questions/{questionId}/answers")]
[ApiController]
public class AnswersController : ControllerBase
{
    private readonly Db db;

    public AnswersController(Db context)
    {
        db = context;
    }

    // GET: api/users/1/quizzes/2/questions/3/answers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Answer>>> GetAnswers(int userId, int quizId, int questionId)
    {
        var question = await db.GetQuestion(userId, quizId, questionId);
        if (question is null)
        {
            return NotFound();
        }

        return question.Answers;
    }

    // GET: api/users/1/quizzes/2/questions/3/answers/4
    [HttpGet("{id}")]
    public async Task<ActionResult<Answer>> GetAnswer(int userId, int quizId, int questionId, int id)
    {
        var question = await db.GetQuestion(userId, quizId, questionId);

        return question?.Answers.FirstOrDefault(a => a.Id == id)
            is Answer answer
                ? answer
                : NotFound();
    }

    // PUT: api/users/1/quizzes/2/questions/3/answers/4
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAnswer(int userId, int quizId, int questionId, int id, Answer answer)
    {
        if (id != answer.Id)
        {
            return BadRequest();
        }

        var question = await db.GetQuestion(userId, quizId, questionId, true);
        if (question is null)
        {
            return NotFound();
        }

        var exists = question.Answers.Any(a => a.Id == id);
        if (!exists)
        {
            return NotFound();
        }

        db.Update(answer);
        await db.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/users/1/quizzes/2/questions/3/answers
    [HttpPost]
    public async Task<ActionResult<Answer>> PostAnswer(int userId, int quizId, int questionId, Answer answer)
    {
        var question = await db.GetQuestion(userId, quizId, questionId);
        if (question is null)
        {
            return NotFound();
        }

        question.Answers.Add(answer);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAnswer), new { userId, quizId, questionId, id = answer.Id }, answer);
    }

    // DELETE: api/users/1/quizzes/2/questions/3/answers/4
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAnswer(int userId, int quizId, int questionId, int id)
    {
        var question = await db.GetQuestion(userId, quizId, questionId);

        if (question?.Answers.FirstOrDefault(a => a.Id == id) is Answer answer)
        {
            question.Answers.Remove(answer);
            await db.SaveChangesAsync();
            return NoContent();
        }

        return NotFound();
    }

}
