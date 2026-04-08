using System.Diagnostics;
using ATS.Infrastructure.Data;
using AssessmentTrackingSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssessmentTrackingSystem.Models;

namespace AssessmentTrackingSystem.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AtsDbContext _context;
    private readonly InAppNotificationService _notificationService;

    public HomeController(
        ILogger<HomeController> logger,
        AtsDbContext context,
        InAppNotificationService notificationService)
    {
        _logger = logger;
        _context = context;
        _notificationService = notificationService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Dashboard()
    {
        var vm = await BuildDashboardViewModelAsync();
        return View(vm);
    }

    public async Task<IActionResult> AssessmentStatus(string status)
    {
        var assessments = await _context.Assessments
            .Include(a => a.Course)
            .Where(a => a.Status == status)
            .OrderBy(a => a.DueDate)
            .ToListAsync();

        ViewBag.SelectedStatus = status;
        return View(assessments);
    }

    private async Task<DashboardViewModel> BuildDashboardViewModelAsync()
    {
        var today = DateTime.Today;
        var nextWeek = today.AddDays(7);

        var upcoming = await _context.Assessments
            .Include(a => a.Course)
            .Where(a => a.DueDate >= today && a.DueDate <= nextWeek)
            .OrderBy(a => a.DueDate)
            .ToListAsync();

        var overdue = await _context.Assessments
            .Include(a => a.Course)
            .Where(a => a.DueDate < today && a.Status != "Completed")
            .OrderBy(a => a.DueDate)
            .ToListAsync();

        var totalCourses = await _context.Courses.CountAsync();
        var totalAssessments = await _context.Assessments.CountAsync();

        var notStarted = await _context.Assessments.CountAsync(a => a.Status == "Not Started");
        var inProgress = await _context.Assessments.CountAsync(a => a.Status == "In Progress");
        var completed = await _context.Assessments.CountAsync(a => a.Status == "Completed");
        var notifications = _notificationService.BuildNotifications(upcoming, overdue, today);

        return new DashboardViewModel
        {
            Notifications = notifications,
            UpcomingAssessments = upcoming,
            OverdueAssessments = overdue,
            TotalCourses = totalCourses,
            TotalAssessments = totalAssessments,
            NotStartedCount = notStarted,
            InProgressCount = inProgress,
            CompletedCount = completed
        };
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
