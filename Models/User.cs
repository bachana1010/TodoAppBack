using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public ICollection<Todo> Todos { get; set; } = new List<Todo>(); 
    }
}
