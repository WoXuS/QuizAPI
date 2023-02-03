using QuizAPI.Interfaces;

namespace QuizAPI.Models;

public class Question : IEntity
{
    public int Id { get; set; }
    public string Text { get; set; } = "";
    public List<Answer> Answers { get; set; } = new();
    
}
