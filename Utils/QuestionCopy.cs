using QuizAPI.Models;

namespace QuizAPI.Utils;

public class QuestionCopy
{
    public int Id { get; set; }
    public string Text { get; set; } = "";
    public List<Answer> Answers { get; set; } = new();
}
