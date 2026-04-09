using ATS.Core.Entities;
using ATS.Infrastructure.Data;
using AssessmentTrackingSystem.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ATS.Tests;

public class AssessmentControllerTests
{
    [Fact]
    public async Task Create_RedirectsToCalendar_WhenAssessmentIsValid()
    {
        await using var context = CreateContext();
        var controller = new AssessmentController(context);
        var dueDate = new DateTime(2026, 4, 10);
        var assessment = new Assessment
        {
            Name = "Assignment 2",
            DueDate = dueDate,
            Status = "Not Started",
            CourseId = 1
        };

        var result = await controller.Create(assessment);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Calendar", redirectResult.ControllerName);
        Assert.Equal("2026-04-10", redirectResult.RouteValues?["weekStart"]);
        Assert.Single(context.Assessments);
    }

    [Fact]
    public async Task Create_ReturnsView_WhenModelStateIsInvalid()
    {
        await using var context = CreateContext();
        var controller = new AssessmentController(context);
        controller.ModelState.AddModelError("Name", "Name is required");

        var assessment = new Assessment
        {
            DueDate = new DateTime(2026, 4, 10),
            CourseId = 1
        };

        var result = await controller.Create(assessment);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(assessment, viewResult.Model);
        Assert.Empty(context.Assessments);
    }

    [Fact]
    public async Task Edit_ReturnsNotFound_WhenRouteIdDoesNotMatchAssessmentId()
    {
        await using var context = CreateContext();
        var controller = new AssessmentController(context);

        var assessment = new Assessment
        {
            Id = 5,
            Name = "Quiz 3",
            DueDate = new DateTime(2026, 4, 11),
            Status = "In Progress",
            CourseId = 1
        };

        var result = await controller.Edit(4, assessment);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteConfirmed_RemovesAssessment_WhenAssessmentExists()
    {
        await using var context = CreateContext();
        context.Assessments.Add(new Assessment
        {
            Id = 7,
            Name = "Midterm",
            DueDate = new DateTime(2026, 4, 15),
            Status = "Not Started",
            CourseId = 1
        });
        await context.SaveChangesAsync();

        var controller = new AssessmentController(context);

        var result = await controller.DeleteConfirmed(7);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Empty(context.Assessments);
    }

    private static AtsDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AtsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new AtsDbContext(options);
        context.Courses.Add(new Course
        {
            Id = 1,
            Name = "Open Source",
            Code = "COMP2152",
            UserId = "test-user"
        });
        context.SaveChanges();

        return context;
    }
}
