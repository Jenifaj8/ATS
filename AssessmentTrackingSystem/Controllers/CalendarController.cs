using ATS.Infrastructure.Data;
using AssessmentTrackingSystem.Models;
using AssessmentTrackingSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssessmentTrackingSystem.Controllers
{
  public class CalendarController : Controller
  {
    private readonly AtsDbContext _context;

    public CalendarController(AtsDbContext context)
    {
      _context = context;
    }

    public async Task<IActionResult> Index(DateTime? weekStart)
    {
      var selectedDate = weekStart?.Date ?? DateTime.Today;
      var mondayOffset = ((int)selectedDate.DayOfWeek + 6) % 7;
      var normalizedWeekStart = selectedDate.AddDays(-mondayOffset);
      var normalizedWeekEnd = normalizedWeekStart.AddDays(7);

      var assessments = await _context.Assessments
        .Include(a => a.Course)
        .Where(a => a.DueDate >= normalizedWeekStart && a.DueDate < normalizedWeekEnd)
        .OrderBy(a => a.DueDate)
        .ThenBy(a => a.Name)
        .ToListAsync();

      var groupedAssessments = assessments
        .GroupBy(a => a.DueDate.Date)
        .ToDictionary(
          group => group.Key,
          group => group.Select(a => new CalendarAssessmentItemViewModel
          {
            Id = a.Id,
            Title = a.Name,
            CourseName = a.Course?.Name ?? "No course",
            Status = a.Status,
            Category = CalendarItemTypeMapper.GetCategoryLabel(a.Name),
            AccentClass = CalendarItemTypeMapper.GetAccentClass(a.Name),
            DueDate = a.DueDate
          }).ToList());

      var model = new CalendarViewModel
      {
        WeekStart = normalizedWeekStart,
        Days = Enumerable.Range(0, 7)
          .Select(offset =>
          {
            var date = normalizedWeekStart.AddDays(offset);

            return new CalendarDayViewModel
            {
              Date = date,
              Items = groupedAssessments.TryGetValue(date.Date, out var items) ? items : []
            };
          })
          .ToList()
      };

      return View(model);
    }
  }
}
