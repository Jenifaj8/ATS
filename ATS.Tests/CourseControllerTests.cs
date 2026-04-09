using ATS.Core.Entities;
using ATS.Infrastructure.Data;
using AssessmentTrackingSystem.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ATS.Tests;

public class CourseControllerTests
{
    [Fact]
    public async Task Create_RedirectsToIndex_WhenCourseIsValid()
    {
        await using var context = CreateContext();
        var controller = new CourseController(context);
        var course = new Course
        {
            Name = "Software Testing",
            Code = "COMP2154",
            UserId = "test-user"
        };

        var result = await controller.Create(course);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal(2, context.Courses.Count());
    }

    [Fact]
    public async Task Create_ReturnsView_WhenModelStateIsInvalid()
    {
        await using var context = CreateContext();
        var controller = new CourseController(context);
        controller.ModelState.AddModelError("Name", "Name is required");

        var course = new Course
        {
            Code = "COMP2154",
            UserId = "test-user"
        };

        var result = await controller.Create(course);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(course, viewResult.Model);
        Assert.Single(context.Courses);
    }

    [Fact]
    public async Task Edit_ReturnsNotFound_WhenRouteIdDoesNotMatchCourseId()
    {
        await using var context = CreateContext();
        var controller = new CourseController(context);
        var course = new Course
        {
            Id = 5,
            Name = "Open Source",
            Code = "COMP2152",
            UserId = "test-user"
        };

        var result = await controller.Edit(4, course);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteConfirmed_RedirectsToIndex_WhenCourseDoesNotExist()
    {
        await using var context = CreateContext();
        var controller = new CourseController(context);

        var result = await controller.DeleteConfirmed(999);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Single(context.Courses);
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
