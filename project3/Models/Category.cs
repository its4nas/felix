using System.ComponentModel.DataAnnotations;

namespace project3.Models
{
    public class Category
    {
        [Key]
        public int cat_id { get; set; }
        public string name { get; set; }
        
        public ICollection<job>? jobs { get; set; }
    }
}
