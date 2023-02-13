using Newtonsoft.Json;

namespace QuizAPI.Models;

public class AuthEvent
{
    public int Id { get; set; }
    public DateTime Time { get; set; }
    public string Event { get; set; } = "";
    public string UserName { get; set; } = "";
    public bool Succeeded { get; set; }

}
