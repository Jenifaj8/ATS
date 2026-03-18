namespace AssessmentTrackingSystem.Models;

public class CalendarViewModel
{
    public DateTime WeekStart { get; set; }

    public DateTime WeekEnd => WeekStart.AddDays(6);

    public List<CalendarDayViewModel> Days { get; set; } = [];

    public int TotalAssessments => Days.Sum(day => day.Items.Count);

    public string TaskSummaryLabel => TotalAssessments == 1 ? "1 task(s)" : $"{TotalAssessments} tasks";
}

public class CalendarDayViewModel
{
    public DateTime Date { get; set; }

    public List<CalendarAssessmentItemViewModel> Items { get; set; } = [];
}

public class CalendarAssessmentItemViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string CourseName { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string AccentClass { get; set; } = "calendar-item--assignment";

    public DateTime DueDate { get; set; }
}
