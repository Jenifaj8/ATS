using ATS.Core.Entities;

namespace AssessmentTrackingSystem.Models;

public class DashboardViewModel
{
    public List<Assessment> UpcomingAssessments { get; set; } = new();
    public List<Assessment> OverdueAssessments { get; set; } = new();

    public int TotalCourses { get; set; }
    public int TotalAssessments { get; set; }

    public int NotStartedCount { get; set; }
    public int InProgressCount { get; set; }
    public int CompletedCount { get; set; }
}