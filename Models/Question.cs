using System.Text.Json.Serialization;

namespace QuizAPI.Models;

public class Question
{
    public int Id { get; set; }
    public string Text { get; set; } = "";

    [JsonIgnore]
    public int QuizId { get; set; }

    [JsonIgnore]
    public List<Answer> Answers { get; set; } = new();
}
