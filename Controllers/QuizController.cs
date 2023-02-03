using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizAPI.Models;

namespace QuizAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : GenericController<Quiz>
    {
        public QuizController(Db context) : base(context) { }

        //// GET: api/Quiz
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Quiz>>> GetQuizzes()
        //{
        //    return await db.Quizzes.ToListAsync();
        //}

        //// GET: api/Quiz/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Quiz>> GetQuiz(int id)
        //{
        //    return await db.Quizzes
        //        .Include(q => q.Questions)
        //        .ThenInclude(q => q.Answers)
        //        .FirstOrDefaultAsync(q => q.Id == id)
        //        is Quiz quiz
        //            ? Ok(quiz)
        //            : NotFound();
        //}

        //// PUT: api/Quiz/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutQuiz(int id, Quiz quiz)
        //{
        //    var exists = await db.Quizzes.AnyAsync(q => q.Id == id);

        //    if (!exists)
        //    {
        //        return NotFound();
        //    }

        //    db.Update(quiz);
        //    await db.SaveChangesAsync();

        //    return NoContent();
        //}

        //// POST: api/Quiz
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Quiz>> PostQuiz(Quiz quiz)
        //{
        //    db.Quizzes.Add(quiz);
        //    await db.SaveChangesAsync();
        //    return CreatedAtAction(nameof(GetQuiz), new { id = quiz.Id }, quiz);
        //}

        //// DELETE: api/Quiz/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteQuiz(int id)
        //{
        //    if (await db.Quizzes.FindAsync(id) is Quiz quiz)
        //    {
        //        db.Quizzes.Remove(quiz);
        //        await db.SaveChangesAsync();
        //        return Ok();
        //    }

        //    return NotFound();
        //}
    }
}
