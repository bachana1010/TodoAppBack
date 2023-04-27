using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TodoApp.Models
{
 public class Todo
{
    public int Id { get; set; }
    public string Task { get; set; } = string.Empty;
    public bool? Status { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; } 
}

}
