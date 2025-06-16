using ComplaintSystem.Models;

namespace ComplaintSystem.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalComplaints { get; set; }
        public int ProcessingComplaints { get; set; }
        public int AssignedComplaints { get; set; }
        public int TotalComplaintsClosed { get; set; }
        public int DelayedComplaints { get; set; }
        public int TotalUsers { get; set; }
        public int Specialists { get; set; }


        public List<Complaint> RecentComplaints { get; set; }
        public List<Assignment> RecentAssignments { get; set; }
        public List<User> RecentUsers { get; set; }
    }
}
