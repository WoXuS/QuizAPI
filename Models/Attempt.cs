namespace QuizAPI.Models;

public class Attempt
{
    public int Id { get; set; }
    public int QuizId { get; set; }
    public int[] QuestionOrder { get; set; } = Array.Empty<int>();
    public int[] ChosenAnswers { get; set; } = Array.Empty<int>();
    public bool IsOpen { get; set; }
}
