using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            return _context.Users.Include(u => u.Todos).ToList();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public ActionResult<User> GetUser(int id)
        {
            var user = _context.Users.Include(u => u.Todos).FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST: api/Users
        [HttpPost]
        public ActionResult<User> RegisterUser(User user)
        {


                var existingEmail = _context.Users.FirstOrDefault(u => u.Email == user.Email);
                    Console.WriteLine($"Existing email: {existingEmail}");

                var existingUsername = _context.Users.FirstOrDefault(u => u.Username == user.Username);

                if (existingEmail != null)
                {
                    // Return an error message
                    return BadRequest(new { error = "Email already registered" });
                }

                if (existingUsername != null)
                {
                    // Return an error message
                    return BadRequest(new { error = "Username already taken" });
                }

                _context.Users.Add(user);
                _context.SaveChanges();

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // Other methods (PUT, DELETE, etc.) 

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
