namespace QuizAPI.Models;

public class Quiz
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public bool IsOpen { get; set; } = false;
    public List<Question> Questions { get; set; } = new();
}
