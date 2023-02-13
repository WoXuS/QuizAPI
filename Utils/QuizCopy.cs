namespace QuizAPI.Utils;

public class QuizCopy
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string UserId { get; set; } = "";
    public List<QuestionCopy> Questions { get; set; } = new();
}
