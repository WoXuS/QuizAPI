namespace QuizAPI.Models;

public class Question
{
    public int Id { get; set; }
    public string Text { get; set; } = "";
    public bool IsOpen { get; set; } = false;
    public List<Answer> Answers { get; set; } = new();
}
