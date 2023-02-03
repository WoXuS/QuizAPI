using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizAPI.Interfaces;
using QuizAPI.Models;

namespace QuizAPI.Controllers;

public abstract class GenericController<T> : ControllerBase where T : class, IEntity
{
    protected readonly Db db;

    public GenericController(Db context)
    {
        db = context;
    }

    // GET: api/Quiz
    [HttpGet]
    public async Task<ActionResult<IEnumerable<T>>> GetAll()
    {
        return await db.Set<T>().ToListAsync();
    }

    // GET: api/Quiz/5
    [HttpGet("{id}")]
    public async Task<ActionResult<T>> GetOne(int id)
    {
        return await db.Set<T>().FindAsync(id)
            is T t
                ? Ok(t)
                : NotFound();
    }

    // PUT: api/Quiz/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, T t)
    {
        var exists = await db.Set<T>().AnyAsync(t => t.Id == id);

        if (!exists)
        {
            return NotFound();
        }

        db.Update(t);
        await db.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/Quiz
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Quiz>> Post(T t)
    {
        db.Set<T>().Add(t);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetOne), new { id = t.Id }, t);
    }

    // DELETE: api/Quiz/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuiz(int id)
    {
        if (await db.Quizzes.FindAsync(id) is T t)
        {
            db.Set<T>().Remove(t);
            await db.SaveChangesAsync();
            return NoContent();
        }

        return NotFound();
    }

}
