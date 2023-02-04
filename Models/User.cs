namespace QuizAPI.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public List<Quiz> Quizzes { get; set; } = new List<Quiz>();
}
