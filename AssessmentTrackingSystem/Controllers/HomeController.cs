using System.Diagnostics;
using ATS.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssessmentTrackingSystem.Models;

namespace AssessmentTrackingSystem.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AtsDbContext _context;

    public HomeController(ILogger<HomeController> logger, AtsDbContext context)
    {
        _logger = logger;
        _context = context;
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

        return new DashboardViewModel
        {
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
