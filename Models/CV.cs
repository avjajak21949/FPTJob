using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace FPTJob.Models
{
    public class CV
    {
        public int CVID { get; set; }
        public string Description { get; set; }
        public string file { get; set; }
        public string ApplicantId { get; set; } // Id của người tìm việc nộp CV
        public IdentityUser Applicant { get; set; }
        public bool? IsApproved { get; set; } // null = chưa duyệt, true = chấp nhận, false = từ chối
        public ICollection<JobCV> JobCV { get; set; }
    }
}
