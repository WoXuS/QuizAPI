namespace QuizAPI.Utils;

public record UserCredentialsDto(
        string UserName = "",
        string Email = "",
        string Password = "",
        string ConfirmPassword = ""
        );

public record RefreshDto(
    string Token = "",
    Guid RefreshToken = default
    );

public record TokenPair(
    string Token = "",
    string RefreshToken = ""
    );

public record AuthResult(bool Succeeded, TokenPair? Tokens, IEnumerable<string>? Errors)
{
    public static implicit operator AuthResult(TokenPair tokens)
    {
        return new AuthResult(true, tokens, null);
    }

    public static implicit operator AuthResult(string[]? errors)
    {
        return new AuthResult(false, null, errors);
    }
}