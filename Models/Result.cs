namespace QuizAPI.Models;

public class Result
{
    public int Id { get; set; }
    public DateTime Submitted { get; set; }
    public int ChosenCorrectAnswers { get; set; }
    public int ChosenIncorrectAnswers { get; set; }
    public int AllCorrectAnswers { get; set; }

    public decimal SuccessRatio { get => (decimal)ChosenCorrectAnswers / AllCorrectAnswers; }

}
