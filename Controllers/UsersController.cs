using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizAPI.Models;

namespace QuizAPI.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly Db db;

    public UsersController(Db context)
    {
        db = context;
    }

    // GET: api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await db.Users.ToListAsync();
    }

    // GET: api/users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        return await db.Users.FindAsync(id)
            is User user
                ? user
                : NotFound();
    }

    // PUT: api/users/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(int id, User user)
    {
        if (id != user.Id)
        {
            return BadRequest();
        }

        var exists = await db.Users.AnyAsync(u => u.Id == id);
        if (!exists)
        {
            return NotFound();
        }

        db.Update(user);
        await db.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/users
    [HttpPost]
    public async Task<ActionResult<User>> PostUser(User user)
    {
        db.Users.Add(user);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    // DELETE: api/users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        if (await db.Users.FindAsync(id) is User user)
        {
            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return NoContent();
        }

        return NotFound();
    }

}
