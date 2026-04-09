using ATS.Core.Entities;
using AssessmentTrackingSystem.Services;

namespace ATS.Tests;

public class InAppNotificationServiceTests
{
    private readonly InAppNotificationService _service = new();

    [Fact]
    public void BuildNotifications_ReturnsOverdueNotification_WhenPastDueAssessmentsExist()
    {
        var today = new DateTime(2026, 4, 8);
        var overdueAssessments = new[]
        {
            CreateAssessment("Midterm", "COMP2152", today.AddDays(-1), "Not Started"),
            CreateAssessment("Quiz 3", "COMP2133", today.AddDays(-2), "In Progress")
        };

        var notifications = _service.BuildNotifications(Array.Empty<Assessment>(), overdueAssessments, today);

        var overdueNotification = Assert.Single(notifications);
        Assert.Equal("overdue", overdueNotification.Type);
        Assert.Equal("2 assessments are overdue", overdueNotification.Title);
        Assert.Equal("See the full overdue list in the section below.", overdueNotification.Message);
        Assert.Equal("bi bi-alarm-fill", overdueNotification.IconClass);
    }

    [Fact]
    public void BuildNotifications_ReturnsTodayAndTomorrowNotifications_ForPendingUpcomingAssessments()
    {
        var today = new DateTime(2026, 4, 8);
        var upcomingAssessments = new[]
        {
            CreateAssessment("Lab Test", "COMP2154", today, "Not Started"),
            CreateAssessment("Assignment 2", "COMP2080", today.AddDays(1), "In Progress"),
            CreateAssessment("Final Project", "COMP2154", today.AddDays(4), "Not Started")
        };

        var notifications = _service.BuildNotifications(upcomingAssessments, Array.Empty<Assessment>(), today);

        Assert.Equal(2, notifications.Count);

        var todayNotification = notifications.Single(n => n.Type == "today");
        Assert.Equal("1 assessment is due today", todayNotification.Title);
        Assert.Equal("Check the upcoming section below for the full list.", todayNotification.Message);
        Assert.Equal("bi bi-alarm-fill", todayNotification.IconClass);

        var tomorrowNotification = notifications.Single(n => n.Type == "tomorrow");
        Assert.Equal("1 assessment is due tomorrow", tomorrowNotification.Title);
        Assert.Equal("Check the upcoming section below for the full list.", tomorrowNotification.Message);
        Assert.Equal("bi bi-clock-fill", tomorrowNotification.IconClass);
    }

    [Fact]
    public void BuildNotifications_DoesNotCreateTodayOrTomorrowNotifications_ForCompletedAssessments()
    {
        var today = new DateTime(2026, 4, 8);
        var upcomingAssessments = new[]
        {
            CreateAssessment("Completed Today", "COMP2154", today, "Completed"),
            CreateAssessment("Completed Tomorrow", "COMP2154", today.AddDays(1), "Completed")
        };

        var notifications = _service.BuildNotifications(upcomingAssessments, Array.Empty<Assessment>(), today);

        Assert.Empty(notifications);
    }

    [Fact]
    public void BuildNotifications_ReturnsEmpty_WhenNoMatchingAssessmentsExist()
    {
        var today = new DateTime(2026, 4, 8);
        var upcomingAssessments = new[]
        {
            CreateAssessment("Essay", "COMP2154", today.AddDays(3), "Not Started"),
            CreateAssessment("Project Demo", "COMP2080", today.AddDays(6), "In Progress")
        };

        var notifications = _service.BuildNotifications(upcomingAssessments, Array.Empty<Assessment>(), today);

        Assert.Empty(notifications);
    }

    [Fact]
    public void BuildNotifications_ReturnsExpectedNotifications_ForMixedAssessmentScenario()
    {
        var today = new DateTime(2026, 4, 8);
        var upcomingAssessments = new[]
        {
            CreateAssessment("Lab Test", "COMP2154", today, "Not Started"),
            CreateAssessment("Assignment 2", "COMP2080", today.AddDays(1), "In Progress"),
            CreateAssessment("Quiz 4", "COMP2133", today.AddDays(1), "Not Started"),
            CreateAssessment("Completed Today", "COMP2154", today, "Completed"),
            CreateAssessment("Project Demo", "COMP2080", today.AddDays(5), "Not Started")
        };
        var overdueAssessments = new[]
        {
            CreateAssessment("Midterm", "COMP2152", today.AddDays(-1), "Not Started")
        };

        var notifications = _service.BuildNotifications(upcomingAssessments, overdueAssessments, today);

        Assert.Equal(3, notifications.Count);

        var overdueNotification = notifications.Single(n => n.Type == "overdue");
        Assert.Equal("1 assessment is overdue", overdueNotification.Title);

        var todayNotification = notifications.Single(n => n.Type == "today");
        Assert.Equal("1 assessment is due today", todayNotification.Title);

        var tomorrowNotification = notifications.Single(n => n.Type == "tomorrow");
        Assert.Equal("2 assessments are due tomorrow", tomorrowNotification.Title);
    }

    private static Assessment CreateAssessment(string name, string courseCode, DateTime dueDate, string status)
    {
        return new Assessment
        {
            Name = name,
            DueDate = dueDate,
            Status = status,
            Course = new Course
            {
                Code = courseCode,
                Name = courseCode
            }
        };
    }
}
