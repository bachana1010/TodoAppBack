using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Dtos;
using Microsoft.AspNetCore.Authorization;
using TodoApp.Models;
using System.Threading.Tasks;
using System.Security.Claims;

namespace TodoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TodosController(AppDbContext context)
        {
            _context = context;
        }


        // GET: api/Todos
        [HttpGet]
        public ActionResult<IEnumerable<TodoDto>> GetTodos()
        {
            var todos = _context.Todos.Include(t => t.User).ToList();

            var todoDtos = todos.Select(todo => new TodoDto
            {
                Id = todo.Id,
                Task = todo.Task,
                Status = todo.Status,
                UserId = todo.UserId,
                User = new UserDto { Id = todo.User.Id, Username = todo.User.Username }
            }).ToList();

            return todoDtos;
        }



        // GET: api/Todos/5
        [HttpGet("{id}")]
        public ActionResult<Todo> GetTodo(int id)
        {
            var todo = _context.Todos.Include(t => t.User).FirstOrDefault(t => t.Id == id);

            if (todo == null)
            {
                return NotFound();
            }

            return todo;
        }


        // PUT: api/Todos/5
        [HttpPut("{id}")]
        public IActionResult UpdateTodo(int id, Todo todo)
        {
            if (id != todo.Id)
            {
                return BadRequest();
            }

            _context.Entry(todo).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        [Authorize]

        // POST: api/Todos
            [Authorize]
        // POST: api/Todos
        [HttpPost]
        public async Task<ActionResult<Todo>> AddTodo(Todo todo)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId;
            if (!int.TryParse(userIdClaim.Value, out userId))
            {
                return BadRequest("Invalid UserId in token");
            }

            //  exists in the Users table
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            todo.UserId = userId;
            todo.Status = todo.Status ?? false; 
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTodo", new { id = todo.Id }, todo);
        }


        

        // DELETE: api/Todos/5
        [HttpDelete("{id}")]
        public IActionResult DeleteTodo(int id)
        {
            var todo = _context.Todos.Find(id);
            if (todo == null)
            {
                return NotFound();
            }

            _context.Todos.Remove(todo);
            _context.SaveChanges();

            return NoContent();
        }

        private bool TodoExists(int id)
        {
            return _context.Todos.Any(e => e.Id == id);
        }
        


        // GET: api/Todos/ByUser
[Authorize]
[HttpGet("ByUser", Name = "GetTodosByUser")] 
public ActionResult<IEnumerable<TodoDto>> GetTodosByUser()
{
    var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
    if (userIdClaim == null)
    {
        return Unauthorized();
    }

    int userId;
    if (!int.TryParse(userIdClaim.Value, out userId))
    {
        return BadRequest("Invalid UserId in token");
    }

    var todos = _context.Todos.Include(t => t.User).Where(t => t.UserId == userId).ToList();

    var todoDtos = todos.Select(todo => new TodoDto
    {
        Id = todo.Id,
        Task = todo.Task,
        Status = todo.Status,
        UserId = todo.UserId,
        User = new UserDto { Id = todo.User.Id, Username = todo.User.Username }
    }).ToList();

    return todoDtos;
}

    }
    
}
