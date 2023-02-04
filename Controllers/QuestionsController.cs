using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizAPI.Models;

namespace QuizAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuestionsController : ControllerBase
{
    private readonly Db _context;

    public QuestionsController(Db context)
    {
        _context = context;
    }

    // GET: api/Question
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Question>>> GetQuestions()
    {
        return await _context.Questions.ToListAsync();
    }

    // GET: api/Question/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Question>> GetQuestion(int id)
    {
        return await _context.Questions
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == id)
            is Question question
                ? Ok(question)
                : NotFound();
    }

    // PUT: api/Question/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutQuestion(int id, Question question)
    {
        if (id != question.Id)
        {
            return BadRequest();
        }

        var exists = await _context.Questions.AnyAsync(q => q.Id == id);
       if (!exists)
        {
            return NotFound();
        }

        _context.Update(question);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/Question
    [HttpPost]
    public async Task<ActionResult<Question>> PostQuestion(Question question)
    {
        _context.Questions.Add(question);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, question);
    }

    // DELETE: api/Question/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuestion(int id)
    {
        if (await _context.Questions.FindAsync(id) is Question question)
        {
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        return NotFound();
    }

}
