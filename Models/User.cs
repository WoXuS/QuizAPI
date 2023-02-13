using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace QuizAPI.Models;

public class User : IdentityUser
{

    [JsonIgnore]
    public List<Quiz> Quizzes { get; set; } = new List<Quiz>();

    [JsonIgnore]
    public List<Attempt> Attempts { get; set; } = new List<Attempt>();
}
