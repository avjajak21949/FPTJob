using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FPTJob.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public ICollection<Job> Job { get; set; }
    }
}
