using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizAPI.Attributes;
using QuizAPI.Models;
using QuizAPI.Utils;

namespace QuizAPI.Controllers;

[AuthorizeJwt(Roles = "Admin")]
[Route("api/admin")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly Db db;

    public AdminController(Db context)
    {
        db = context;
    }

    // GET: api/admin/log
    [HttpGet("log")]
    public async Task<ActionResult<IEnumerable<AuthEvent>>> GetAuthEvents()
        => await db.AuthEvents.ToListAsync();

}
