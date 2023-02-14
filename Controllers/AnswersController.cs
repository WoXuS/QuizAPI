using Microsoft.AspNetCore.Mvc;
using QuizAPI.Attributes;
using QuizAPI.Models;
using QuizAPI.Utils;

namespace QuizAPI.Controllers;

[AuthorizeJwt]
[Route("api/quizzes/{quizId}/questions/{questionId}/answers")]
[ApiController]
public class AnswersController : ControllerBase
{
    private readonly Db db;

    public AnswersController(Db context)
    {
        db = context;
    }

    // GET: api/quizzes/2/questions/3/answers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Answer>>> GetAnswers(int quizId, int questionId)
    {
        var userId = User.GetUserID();
        var tuple = await db.GetQuizAndQuestionWithAnswers(userId!, quizId, questionId);

        if (tuple is null)
        {
            return NotFound();
        }

        var question = tuple.Value.Item2;
        return question.Answers;
    }

    // GET: api/quizzes/2/questions/3/answers/4
    [HttpGet("{id}")]
    public async Task<ActionResult<Answer>> GetAnswer(int quizId, int questionId, int id)
    {
        var userId = User.GetUserID();
        var tuple = await db.GetQuizAndQuestionWithAnswers(userId!, quizId, questionId);

        var question = tuple.GetValueOrDefault().Item2;

        return question?.Answers.FirstOrDefault(a => a.Id == id)
            is Answer answer
                ? answer
                : NotFound();
    }

    // PUT: api/quizzes/2/questions/3/answers/4
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAnswer(int quizId, int questionId, int id, Answer answer)
    {
        if (id != answer.Id)
        {
            return BadRequest();
        }

        var userId = User.GetUserID();
        var tuple = await db.GetQuizAndQuestionWithAnswers(userId!, quizId, questionId);
        if (tuple is null)
        {
            return NotFound();
        }

        var quiz = tuple.Value.Item1;
        if (quiz.IsOpen)
        {
            return Conflict();
        }

        var question = tuple.Value.Item2;
        var existing = question.Answers.FirstOrDefault(a => a.Id == id);
        if (existing is null)
        {
            return NotFound();
        }

        existing.Text = answer.Text;
        existing.IsCorrect = answer.IsCorrect;
        db.Update(existing);
        await db.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/quizzes/2/questions/3/answers
    [HttpPost]
    public async Task<ActionResult<Answer>> PostAnswer(int quizId, int questionId, Answer answer)
    {
        var userId = User.GetUserID();
        var tuple = await db.GetQuizAndQuestionWithAnswers(userId!, quizId, questionId);

        if (tuple is null)
        {
            return NotFound();
        }

        var quiz = tuple.Value.Item1;
        if (quiz.IsOpen)
        {
            return Conflict();
        }

        var question = tuple.Value.Item2;
        db.Attach(question);
        question.Answers.Add(answer);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAnswer), new { quizId, questionId, id = answer.Id }, answer);
    }

    // DELETE: api/quizzes/2/questions/3/answers/4
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAnswer(int quizId, int questionId, int id)
    {
        var userId = User.GetUserID();
        var tuple = await db.GetQuizAndQuestionWithAnswers(userId!, quizId, questionId);

        if (tuple is null)
        {
            return NotFound();
        }

        var quiz = tuple.Value.Item1;
        if (quiz.IsOpen)
        {
            return Conflict();
        }

        var question = tuple.Value.Item2;

        if (question.Answers.FirstOrDefault(a => a.Id == id) is Answer answer)
        {
            db.Answers.Remove(answer);
            await db.SaveChangesAsync();
            return NoContent();
        }

        return NotFound();
    }

}
