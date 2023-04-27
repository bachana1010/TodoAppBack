using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtSettings _jwtSettings;

        public AuthController(AppDbContext context, IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            var existingEmail = _context.Users.FirstOrDefault(u => u.Email == user.Email);
            var existingUsername = _context.Users.FirstOrDefault(u => u.Username == user.Username);

            if (existingEmail != null)
            {
                return BadRequest(new { error = "Email already registered" });
            }

            if (existingUsername != null)
            {
                return BadRequest(new { error = "Username already taken" });
            }

            _context.Users.Add(user);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }


            [HttpPost("login")]
            public IActionResult Login([FromBody] LoginRequest request)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
                if (user == null || user.Password != request.Password)
                {
                    return Unauthorized();
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

                var claims = new[]
                {
                    new Claim("sub", user.Id.ToString()),
                    new Claim("id", user.Id.ToString()),
                    new Claim("username", user.Username),
                    new Claim("email", user.Email)
                };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Ok(new { Token = tokenString });
            }




        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }


        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}
