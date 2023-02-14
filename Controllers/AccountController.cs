using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuizAPI.Attributes;
using QuizAPI.Models;
using QuizAPI.Services;
using QuizAPI.Utils;
using System.IdentityModel.Tokens.Jwt;

namespace QuizAPI.Controllers;

[Route("api/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<User> userManager;
    private readonly Db db;
    private readonly ILogger<AccountController> logger;
    private readonly AuthenticationService service;

    public AccountController(UserManager<User> userManager,
        Db db,
        ILogger<AccountController> logger,
        AuthenticationService service)
    {
        this.userManager = userManager;
        this.db = db;
        this.logger = logger;
        this.service = service;
    }

    private BadRequestObjectResult BadRequest(IdentityResult result)
        => BadRequest(result.Errors.Select(e => e.Description));    
    
    private BadRequestObjectResult BadRequest(IEnumerable<string> errors)
        => BadRequest(new { Errors = errors });

    private ActionResult<TokenPair> Unpack(AuthResult result) => result.Succeeded ? result.Tokens! : BadRequest(new { result.Errors });


    [HttpPost("register")]
    public async Task<ActionResult<TokenPair>> Register(UserCredentialsDto dto)
    {
        var result = await service.RegisterImpl(dto);
        await db.LogAuthEvent("register", dto.UserName, result.Succeeded);

        return Unpack(result);
    }


    [HttpPost("authenticate")]
    public async Task<ActionResult<TokenPair>> Authenticate(string userName, string password)
    {
        var result = await service.AuthenticateImpl(userName, password);
        await db.LogAuthEvent("authenticate", userName, result.Succeeded);

        return Unpack(result);
    }


    [HttpPost("refresh")]
    public async Task<ActionResult<TokenPair>> Refresh(RefreshDto dto)
    {
        var result = await service.RefreshImpl(dto.Token, dto.RefreshToken);

        var userName = "";
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(dto.Token);
            var claimValue = securityToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value;
            userName = claimValue ?? "";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "unable to extract username from JWT");
        }

        await db.LogAuthEvent("refresh", userName, result.Succeeded);

        return Unpack(result);
    }


    [AuthorizeJwt]
    [HttpPatch("invalidate")]
    public async Task<IActionResult> Invalidate(Guid refreshToken)
    {
        var userId = User.GetUserID();
        var (succeeded, errors) = await service.InvalidateImpl(refreshToken, userId!);

        var user = await userManager.FindByIdAsync(userId!);
        await db.LogAuthEvent("invalidate", user!.UserName!, succeeded);

        if (!succeeded)
        {
            return BadRequest(errors!);
        }
        return Ok();
    }


    [AuthorizeJwt]
    [HttpGet("details")]
    public async Task<IActionResult> GetUser()
    {
        var userId = User.GetUserID();
        var user = await userManager.FindByIdAsync(userId!);
        var roles = await userManager.GetRolesAsync(user!);

        var response = new { user!.UserName, user.Email, Roles = roles };

        return Ok(response);
    }


    [AuthorizeJwt]
    [HttpPut("credentials")]
    public async Task<IActionResult> UpdateUser(UserCredentialsDto dto)
    {
        if (dto.Password != dto.ConfirmPassword)
        {
            return BadRequest(new[] { "password doesn't match confirmPassword" });
        }

        var userId = User.GetUserID();
        var currentUser = await userManager.FindByIdAsync(userId!);
        var result = await userManager.SetUserNameAsync(currentUser!, dto.UserName);
        if (!result.Succeeded)
        {
            return BadRequest(result);
        }

        result = await userManager.SetEmailAsync(currentUser!, dto.Email);
        if (!result.Succeeded)
        {
            return BadRequest(result);
        }

        var resetToken = await userManager.GeneratePasswordResetTokenAsync(currentUser!);
        result = await userManager.ResetPasswordAsync(currentUser!, resetToken, dto.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result);
        }

        return NoContent();
    }


    [HttpGet("username")]
    public async Task<ActionResult<string>> GetUserName(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        return user is null ? NotFound() : user.UserName!;
    }

}