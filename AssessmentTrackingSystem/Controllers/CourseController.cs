using ATS.Core.Entities;
using ATS.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssessmentTrackingSystem.Controllers;

public class CourseController : Controller
{
    private readonly AtsDbContext _context;

    public CourseController(AtsDbContext context)
    {
        _context = context;
    }

    // GET: /Course
    public async Task<IActionResult> Index()
    {
        var courses = await _context.Courses
            .OrderBy(c => c.Name)
            .ToListAsync();

        return View(courses);
    }

    // GET: /Course/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var course = await _context.Courses
            .Include(c => c.Assessments)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            return NotFound();

        return View(course);
    }

    // GET: /Course/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Course/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Course course)
    {
        if (!ModelState.IsValid)
            return View(course);

        _context.Add(course);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /Course/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var course = await _context.Courses.FindAsync(id);
        if (course == null)
            return NotFound();

        return View(course);
    }

    // POST: /Course/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Course course)
    {
        if (id != course.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(course);

        try
        {
            _context.Update(course);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CourseExists(course.Id))
                return NotFound();

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: /Course/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            return NotFound();

        return View(course);
    }

    // POST: /Course/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var course = await _context.Courses
            .Include(c => c.Assessments)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            return RedirectToAction(nameof(Index));

        // If you later configure cascade deletes, this still works fine.
        // For now, if there are assessments, you can decide what to do.
        // We'll keep it simple and remove the course (may fail if FK restricts).
        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private bool CourseExists(int id)
    {
        return _context.Courses.Any(e => e.Id == id);
    }
}