namespace FPTJob.Models
{
    public class JobCV
    {
        public int JobCVID { get; set; }
        public int JobId { get; set; }
        public int CVID { get; set; }
        public bool IsApproved { get; set; } // Cho phép employer approve CV

        // Quan hệ với CV và Job
        public Job Job { get; set; }
        public CV CV { get; set; }
    }
}
