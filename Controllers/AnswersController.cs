using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizAPI.Models;

namespace QuizAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnswersController : ControllerBase
{
    private readonly Db _context;

    public AnswersController(Db context)
    {
        _context = context;
    }

    // GET: api/Answers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Answer>>> GetAnswers()
    {
        return await _context.Answers.ToListAsync();
    }

    // GET: api/Answers/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Answer>> GetAnswer(int id)
    {
        return await _context.Answers.FindAsync(id)
            is Answer answer
                ? Ok(answer)
                : NotFound();
    }

    // PUT: api/Answers/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAnswer(int id, Answer answer)
    {
        if (id != answer.Id)
        {
            return BadRequest();
        }

        var exists = await _context.Answers.AnyAsync(a => a.Id == id);
       if (!exists)
        {
            return NotFound();
        }

        _context.Update(answer);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/Answers
    [HttpPost]
    public async Task<ActionResult<Answer>> PostAnswer(Answer answer)
    {
        _context.Answers.Add(answer);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAnswer), new { id = answer.Id }, answer);
    }

    // DELETE: api/Answers/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAnswer(int id)
    {
        if (await _context.Answers.FindAsync(id) is Answer answer)
        {
            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        return NotFound();
    }

}
