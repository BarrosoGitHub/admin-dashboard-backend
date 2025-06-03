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
            if (request.Username == "admin" && request.Password == "password")
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
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
