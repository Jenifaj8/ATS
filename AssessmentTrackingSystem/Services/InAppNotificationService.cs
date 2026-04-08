using ATS.Core.Entities;
using AssessmentTrackingSystem.Models;

namespace AssessmentTrackingSystem.Services;

public class InAppNotificationService
{
    public List<InAppNotificationViewModel> BuildNotifications(
        IEnumerable<Assessment> upcomingAssessments,
        IEnumerable<Assessment> overdueAssessments,
        DateTime today)
    {
        var notifications = new List<InAppNotificationViewModel>();
        var normalizedToday = today.Date;
        var pendingUpcoming = upcomingAssessments
            .Where(a => !string.Equals(a.Status, "Completed", StringComparison.OrdinalIgnoreCase))
            .ToList();
        var overdueList = overdueAssessments.ToList();
        var dueToday = pendingUpcoming
            .Where(a => a.DueDate.Date == normalizedToday)
            .ToList();
        var dueTomorrow = pendingUpcoming
            .Where(a => a.DueDate.Date == normalizedToday.AddDays(1))
            .ToList();

        if (overdueList.Any())
        {
            notifications.Add(new InAppNotificationViewModel
            {
                Id = $"overdue-{normalizedToday:yyyyMMdd}-{overdueList.Count}",
                Title = overdueList.Count == 1
                    ? "1 assessment is overdue"
                    : $"{overdueList.Count} assessments are overdue",
                Message = "See the full overdue list in the section below.",
                Type = "overdue",
                IconClass = "bi bi-alarm-fill",
                ActionText = "Review overdue work",
                ActionUrl = "#overdue-assessments-panel"
            });
        }

        if (dueToday.Any())
        {
            notifications.Add(new InAppNotificationViewModel
            {
                Id = $"today-{normalizedToday:yyyyMMdd}-{dueToday.Count}",
                Title = dueToday.Count == 1
                    ? "1 assessment is due today"
                    : $"{dueToday.Count} assessments are due today",
                Message = "Check the upcoming section below for the full list.",
                Type = "today",
                IconClass = "bi bi-alarm-fill",
                ActionText = "Open upcoming work",
                ActionUrl = "#upcoming-assessments-panel"
            });
        }

        if (dueTomorrow.Any())
        {
            notifications.Add(new InAppNotificationViewModel
            {
                Id = $"tomorrow-{normalizedToday:yyyyMMdd}-{dueTomorrow.Count}",
                Title = dueTomorrow.Count == 1
                    ? "1 assessment is due tomorrow"
                    : $"{dueTomorrow.Count} assessments are due tomorrow",
                Message = "Check the upcoming section below for the full list.",
                Type = "tomorrow",
                IconClass = "bi bi-clock-fill",
                ActionText = "Check tomorrow's tasks",
                ActionUrl = "#upcoming-assessments-panel"
            });
        }

        return notifications;
    }
}
