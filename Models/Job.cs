using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FPTJob.Models
{
    public class Job
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Range(250, 50000)]
        public int Salary { get; set; }
        [Required]
        public string Place { get; set; }
        [Required]
        public string Time { get; set; }
        public int CategoryID { get; set; }
        public Category Category { get; set; }
        // Thêm EmployerId để liên kết với người dùng
        public string EmployerId { get; set; }
        // EmployerId sẽ liên kết với IdentityUser
        public bool IsApproved { get; set; } = false; // Default is false (not approved)
        public IdentityUser Employer { get; set; }
        public ICollection<JobCV> JobCV { get; set; }

    }
}
