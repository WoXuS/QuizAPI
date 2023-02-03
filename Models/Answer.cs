using QuizAPI.Interfaces;

namespace QuizAPI.Models;

public class Answer : IEntity
{
    public int Id { get; set; }
    public string Text { get; set; } = "";
    public bool IsCorrect { get; set; } = false;
}
