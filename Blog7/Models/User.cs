using System.ComponentModel.DataAnnotations.Schema;
using Blog7.Data;

namespace Blog7.Models;

[Table("User")]
public class User
{  
    [Column("Id")]
    public int Id { get; set; }

    [Column("Username")] public string Username { get; set; } = String.Empty;

    [Column("Password")] public string Password { get; set; } = String.Empty;
    public ICollection<Post> Posts { get; } = new List<Post>();
}