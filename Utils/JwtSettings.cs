namespace QuizAPI.Utils;

public record JwtSettings(string Secret, TimeSpan TokenLifetimeInSeconds);