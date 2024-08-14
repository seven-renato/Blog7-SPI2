using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog7.Models;

public class Post
{
    [Column("Id")]
    public int Id { get; set; }
    [Column("TimeStamp")]
    public DateTime TimeStamp { get; set; }
    [Column("Title")]
    [Required]
    public string Title { get; set; } = String.Empty;
    [Column("Content")]
    [Required]
    public string Content { get; set; } = String.Empty;
    public int UserId { get; set; }
    public User? User { get; set; } = null!;

}