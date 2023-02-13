using System.Security.Claims;

namespace QuizAPI.Utils;

public static class GeneralExtensions
{
    public static string? GetUserID(this ClaimsPrincipal User)
        => User.FindFirstValue("userId");
}
