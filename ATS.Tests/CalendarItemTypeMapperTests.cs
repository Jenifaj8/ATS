using AssessmentTrackingSystem.Services;

namespace ATS.Tests;

public class CalendarItemTypeMapperTests
{
    [Theory]
    [InlineData("quiz 3", "Quiz")]
    [InlineData("final exam", "Exam")]
    [InlineData("midterm test", "Test")]
    [InlineData("assignment 2", "Assignment")]
    [InlineData("project demo", "Project")]
    [InlineData("weekly reflection", "Assessment")]
    public void GetCategoryLabel_ReturnsExpectedCategory_ForCommonAssessmentNames(string assessmentName, string expectedCategory)
    {
        var category = CalendarItemTypeMapper.GetCategoryLabel(assessmentName);

        Assert.Equal(expectedCategory, category);
    }

    [Theory]
    [InlineData("quiz 3", "calendar-item-quiz")]
    [InlineData("final exam", "calendar-item-exam")]
    [InlineData("midterm test", "calendar-item-test")]
    [InlineData("assignment 2", "calendar-item-assignment")]
    [InlineData("project demo", "calendar-item-project")]
    [InlineData("weekly reflection", "calendar-item-assessment")]
    public void GetAccentClass_ReturnsExpectedAccentClass_ForCommonAssessmentNames(string assessmentName, string expectedAccentClass)
    {
        var accentClass = CalendarItemTypeMapper.GetAccentClass(assessmentName);

        Assert.Equal(expectedAccentClass, accentClass);
    }

    [Fact]
    public void GetCategoryLabel_IgnoresExtraSpacesAndUppercaseLetters()
    {
        var category = CalendarItemTypeMapper.GetCategoryLabel("   FINAL EXAM   ");

        Assert.Equal("Exam", category);
    }

    [Fact]
    public void GetAccentClass_ReturnsDefaultAccentClass_WhenNameDoesNotMatchKnownTypes()
    {
        var accentClass = CalendarItemTypeMapper.GetAccentClass("discussion post");

        Assert.Equal("calendar-item-assessment", accentClass);
    }
}
