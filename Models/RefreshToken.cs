namespace QuizAPI.Models;

public class RefreshToken
{
    public Guid Token { get; set; }
    public string JwtId { get; set; } = "";
    public DateTime Creation { get; set; }
    public DateTime Expiration { get; set; }
    public bool Used { get; set; } = false;
    public bool Invalidated { get; set; } = false;
    public string UserId { get; set; } = "";
}
