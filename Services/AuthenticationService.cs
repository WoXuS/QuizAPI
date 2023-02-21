using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using Microsoft.IdentityModel.Tokens;
using QuizAPI.Models;
using QuizAPI.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace QuizAPI.Services;

public class AuthenticationService
{
    private readonly UserManager<User> userManager;
    private readonly SignInManager<User> signInManager;
    private readonly TokenValidationParameters tokenValidationParameters;
    private readonly JwtSettings jwtSettings;
    private readonly Db db;
    private readonly ILogger<AuthenticationService> logger;


    public AuthenticationService(UserManager<User> userManager,
        SignInManager<User> signInManager,
        TokenValidationParameters tokenValidationParameters,
        JwtSettings jwtSettings,
        Db db,
        ILogger<AuthenticationService> logger)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.tokenValidationParameters = tokenValidationParameters;
        this.jwtSettings = jwtSettings;
        this.db = db;
        this.logger = logger;
    }


    private AuthResult Error(string[] errors)
        => new AuthResult(false, null, errors);


    private AuthResult Error(IdentityResult result)
        => result.Errors.Select(e => e.Description).ToArray();


    public async Task<AuthResult> RegisterImpl(UserCredentialsDto dto)
    {
        if (dto.Password != dto.ConfirmPassword)
        {
            return Error(new[] { "password doesn't match confirmPassword" });
        }

        if (!MailAddress.TryCreate(dto.Email, out var _))
        {
            return Error(new[] { "email is not a valid email addres" });
        }

        var user = new User()
        {
            Email = dto.Email,
            UserName = dto.UserName
        };
        var result = await userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            return Error(result);
        }

        user = await userManager.FindByNameAsync(dto.UserName);
        result = await userManager.AddToRoleAsync(user!, "User");
        if (!result.Succeeded)
        {
            return Error(result);
        }

        return await GenerateToken(user!);
    }


    public async Task<AuthResult> AuthenticateImpl(string userName, string password)
    {
        var user = await userManager.FindByNameAsync(userName);

        if (user == null)
        {
            return Error(new[] { "user doesn't exist" });
        }

        var isPasswordValid = await signInManager
            .CheckPasswordSignInAsync(user, password, false);

        if (!isPasswordValid.Succeeded)
        {
            if (isPasswordValid.IsNotAllowed)
            {
                return Error(new[] { "email not confirmed" });
            }

            return Error(new[] { "password invalid" });
        }

        return await GenerateToken(user);
    }


    private async Task<TokenPair> GenerateToken(User user)
    {
        var roles = await userManager.GetRolesAsync(user);

        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("userId", user.Id),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            }),
            Expires = DateTime.Now.Add(jwtSettings.TokenLifetimeInSeconds),
            SigningCredentials = new(new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature)
        };

        foreach (var r in roles)
        {
            descriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, r));
        }

        var token = handler.CreateToken(descriptor);

        var refreshToken = new RefreshToken
        {
            JwtId = token.Id,
            UserId = user.Id,
            Creation = DateTime.UtcNow,
            Expiration = DateTime.UtcNow.AddDays(1)
        };
        db.RefreshTokens.Add(refreshToken);
        await db.SaveChangesAsync();

        return new TokenPair(handler.WriteToken(token), refreshToken.Token.ToString());
    }


    public async Task<AuthResult> RefreshImpl(string token, Guid refreshToken)
    {
        var validatedToken = GetPrincipal(token);
        if (validatedToken is null)
        {
            return Error(new[] { "JWT validation failed;" });
        }

        var expiryDateUnix = long.Parse(validatedToken.Claims
            .Single(x => x.Type == JwtRegisteredClaimNames.Exp)
            .Value
            );

        var expiryDateTimeUtc =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            .AddSeconds(expiryDateUnix);

        if (expiryDateTimeUtc > DateTime.UtcNow)
        {
            return Error(new[] { "JWT token hasn't expired yet" });
        }

        var jti = validatedToken.Claims
            .Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
        var storedRefreshToken = await db.RefreshTokens
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Token == refreshToken);

        if (storedRefreshToken is null)
        {
            return Error(new[] { "This refresh token does not exist" });
        }

        if (DateTime.UtcNow > storedRefreshToken.Expiration)
        {
            return Error(new[] { "This refresh token has expired" });
        }

        if (storedRefreshToken.Invalidated)
        {
            return Error(new[] { "This refresh token has been invalidated" });
        }

        if (storedRefreshToken.Used)
        {
            return Error(new[] { "This refresh token has been used" });
        }

        if (storedRefreshToken.JwtId != jti)
        {
            return Error(new[] { "This refresh token does not match this JWT" });
        }

        storedRefreshToken.Used = true;
        db.RefreshTokens.Update(storedRefreshToken);
        await db.SaveChangesAsync();

        var user = await userManager.FindByIdAsync(
            validatedToken.Claims.Single(x => x.Type == "userId").Value
            );
        return await GenerateToken(user!);
    }


    private ClaimsPrincipal? GetPrincipal(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters,
                out var securityToken);
            return principal;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "JWT validation failed");
            return null;
        }
    }


    public async Task<(bool succeeded, string[]? errors)> InvalidateImpl(Guid refreshToken, string userId)
    {
        var storedRefreshToken = await db.RefreshTokens
            .SingleOrDefaultAsync(x => x.Token == refreshToken);

        if (storedRefreshToken is null)
        {
            return (false, new[] { "This refresh token does not exist" });
        }

        if (storedRefreshToken.UserId != userId)
        {
            return (false, new[] { "This refresh token does not belong to this user" });
        }

        if (DateTime.UtcNow > storedRefreshToken.Expiration)
        {
            return (false, new[] { "This refresh token has expired" });
        }

        if (storedRefreshToken.Invalidated)
        {
            return (false, new[] { "This refresh token has been invalidated" });
        }

        if (storedRefreshToken.Used)
        {
            return (false, new[] { "This refresh token has been used" });
        }

        storedRefreshToken.Invalidated = true;
        await db.SaveChangesAsync();
        return (true, null);
    }

    public async Task<string> GetEmailConfirmationToken(string userName)
    {
        var user = await userManager.FindByNameAsync(userName);

        if (user is null)
        {
            throw new InvalidOperationException($"user with userName '{userName}' does not exist");
        }

        return await userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<bool> ConfirmEmailAddress(string userName, string emailConfirmationToken)
    {
        var user = await userManager.FindByNameAsync(userName);

        if (user is null)
        {
            throw new InvalidOperationException($"user with userName '{userName}' does not exist");
        }

        var result = await userManager.ConfirmEmailAsync(user, emailConfirmationToken);
        return result.Succeeded;
    }
}
