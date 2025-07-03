using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OPTConfigurator.Models;

namespace OPTConfigurator.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private static readonly HashSet<string> BlacklistedTokens = new HashSet<string>();
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var configUsername = _configuration["Username"];
            var configPassword = _configuration["Password"];
            var jwtKey = _configuration["Jwt:Key"];
            
            if (string.IsNullOrEmpty(configUsername) || string.IsNullOrEmpty(configPassword) || string.IsNullOrEmpty(jwtKey))
            {
                return StatusCode(500, "Authentication configuration is missing.");
            }
            
            if (request.Username == configUsername && request.Password == configPassword)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(jwtKey);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, request.Username)
                    }),
                    Expires = DateTime.UtcNow.AddHours(20),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return Ok(new LoginResponse
                {
                    Token = tokenHandler.WriteToken(token),
                    ExpiresAt = tokenDescriptor.Expires.Value
                });
            }
            return Unauthorized();
        }


        [HttpPost("logout")]
        public IActionResult Logout([FromBody] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new { Message = "Token is required." });
            }

            // Add the token to the blacklist
            BlacklistedTokens.Add(token);
            return Ok(new { Message = "Logged out successfully. Token is now blacklisted." });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.CurrentPassword) || string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest(new { Message = "Current password and new password are required." });
            }

            var configUsername = _configuration["Username"];
            var configPassword = _configuration["Password"];

            if (string.IsNullOrEmpty(configUsername) || string.IsNullOrEmpty(configPassword))
            {
                return StatusCode(500, "Authentication configuration is missing.");
            }

            // Verify current password
            if (request.CurrentPassword != configPassword)
            {
                return Unauthorized(new { Message = "Current password is incorrect." });
            }

            try
            {
                // Update the auth.json file
                var authConfig = new
                {
                    Username = configUsername,
                    Password = request.NewPassword
                };

                var json = System.Text.Json.JsonSerializer.Serialize(authConfig, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });

                await System.IO.File.WriteAllTextAsync("auth.json", json);

                // Reload configuration
                (_configuration as IConfigurationRoot)?.Reload();

                return Ok(new { Message = "Password changed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to update password.", Error = ex.Message });
            }
        }

        [HttpGet("validate")]
        public IActionResult ValidateToken([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token) || token.Count(c => c == '.') != 2)
            {
                return Ok(new { Valid = false, Error = "Malformed or missing token." });
            }

            //Blacklisted
            if (BlacklistedTokens.Contains(token))
            {
                return Ok(new { Valid = false, Error = "Token is blacklisted." });
            }

            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                return Ok(new { Valid = false, Error = "JWT key is not configured." });
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtKey);
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true,
                    ValidateLifetime = true
                }, out SecurityToken validatedToken);
                var jwtToken = validatedToken as JwtSecurityToken;
                var username = principal?.Identity?.Name;
                var exp = jwtToken?.ValidTo;
                return Ok(new { Valid = true, Username = username, ExpiresAt = exp });
            }
            catch (Exception ex)
            {
                return Ok(new { Valid = false, Error = ex.Message });
            }
        }
    }
}
