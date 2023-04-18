using System.ComponentModel.DataAnnotations;

namespace project3.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public string? name { get; set; } 
        public string? email { get; set; } 
        public string? message { get; set; } 
    }
}
