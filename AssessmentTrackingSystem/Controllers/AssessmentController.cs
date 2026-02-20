using ATS.Core.Entities;
using ATS.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AssessmentTrackingSystem.Controllers;

public class AssessmentController : Controller
{
    private readonly AtsDbContext _context;

    public AssessmentController(AtsDbContext context)
    {
        _context = context;
    }

    // GET: /Assessment
    public async Task<IActionResult> Index()
    {
        var assessments = await _context.Assessments
            .Include(a => a.Course)
            .OrderBy(a => a.DueDate)
            .ToListAsync();

        return View(assessments);
    }

    // GET: /Assessment/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var assessment = await _context.Assessments
            .Include(a => a.Course)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (assessment == null) return NotFound();

        return View(assessment);
    }

    // GET: /Assessment/Create
    public async Task<IActionResult> Create()
    {
        await PopulateCoursesDropDownList();
        return View();
    }

    // POST: /Assessment/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Assessment assessment)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCoursesDropDownList(assessment.CourseId);
            return View(assessment);
        }

        _context.Add(assessment);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /Assessment/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var assessment = await _context.Assessments.FindAsync(id);
        if (assessment == null) return NotFound();

        await PopulateCoursesDropDownList(assessment.CourseId);
        return View(assessment);
    }

    // POST: /Assessment/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Assessment assessment)
    {
        if (id != assessment.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            await PopulateCoursesDropDownList(assessment.CourseId);
            return View(assessment);
        }

        try
        {
            _context.Update(assessment);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AssessmentExists(assessment.Id)) return NotFound();
            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: /Assessment/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var assessment = await _context.Assessments
            .Include(a => a.Course)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (assessment == null) return NotFound();

        return View(assessment);
    }

    // POST: /Assessment/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var assessment = await _context.Assessments.FindAsync(id);
        if (assessment != null)
        {
            _context.Assessments.Remove(assessment);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateCoursesDropDownList(object? selectedCourse = null)
    {
        var courses = await _context.Courses
            .OrderBy(c => c.Name)
            .ToListAsync();

        ViewBag.CourseId = new SelectList(courses, "Id", "Name", selectedCourse);
    }

    private bool AssessmentExists(int id)
    {
        return _context.Assessments.Any(e => e.Id == id);
    }
}