using System.Text.Json.Serialization;

namespace QuizAPI.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    [JsonIgnore]
    public List<Quiz> Quizzes { get; set; } = new List<Quiz>();

    [JsonIgnore]
    public List<Attempt> Attempts { get; set; } = new List<Attempt>();
}
