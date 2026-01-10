using System.Security.Claims;
using DreamShelf.API.Common.Dtos.Request;
using DreamShelf.API.Common.Interfaces;
using DreamShelf.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DreamShelf.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController (IAuthService authService, IUserService userService) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly IUserService _userService = userService;
    
    private string DeviceType => Request.Headers.UserAgent.ToString();
    
    [HttpPost("register")]
    public async Task <IActionResult> Register([FromBody] UserAuthRequestDto request)
    {
        var userExists = await _userService.UserExists(request.Email);
        
        if(userExists)
        {
            return BadRequest("User with this email already exists");
        }
        
        var token = await _userService.RegisterUser(request.Email, request.Password, DeviceType);
        
        SetRefreshTokenCookie(token.RefreshToken, token.RefreshTokenExpiration);
        
        return Ok(new { AccessToken = token.AccessToken });
    }
    
    [HttpPost("login")]
    public async Task <IActionResult> Login([FromBody] UserAuthRequestDto request)
    {
        var user = await _userService.GetUserByEmail(request.Email);
        
        if(user?.PasswordHash is null || !_authService.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid email or password");
        }
        
        var token = await _userService.LogInUser(user, DeviceType);
        
        SetRefreshTokenCookie(token.RefreshToken, token.RefreshTokenExpiration);
        
        return Ok(new { AccessToken = token.AccessToken });
    }
    
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        string? idValue = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        if (idValue == null) return Unauthorized();

        int id = int.Parse(idValue);
        
        await _userService.LogoutUser(id, DeviceType);
        
        Response.Cookies.Delete("refreshToken");
        
        return Ok("Logged out");
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            return Unauthorized("No refresh token provided");
        
        var userRefreshToken = await _userService.GetUserRefreshToken(refreshToken);
        
        if (userRefreshToken == null || userRefreshToken.Expiration <= DateTime.UtcNow)
            return Unauthorized("Invalid or expired refresh token");
        
        var token =  await _userService.RefreshUser(userRefreshToken);
        
        SetRefreshTokenCookie(token.RefreshToken, token.RefreshTokenExpiration);

        return Ok(new { AccessToken = token.AccessToken });
    }
    
    private void SetRefreshTokenCookie(string token, DateTime expiration)
    {
        Response.Cookies.Append("refreshToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = expiration
        });
    }
}
