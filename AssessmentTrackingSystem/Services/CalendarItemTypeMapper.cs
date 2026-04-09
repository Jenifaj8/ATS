namespace AssessmentTrackingSystem.Services;

public static class CalendarItemTypeMapper
{
    public static string GetCategoryLabel(string name)
    {
        var normalizedName = NormalizeName(name);

        if (normalizedName.Contains("exam")) return "Exam";
        if (normalizedName.Contains("quiz")) return "Quiz";
        if (normalizedName.Contains("test")) return "Test";
        if (normalizedName.Contains("assignment")) return "Assignment";
        if (normalizedName.Contains("project")) return "Project";

        return "Assessment";
    }

    public static string GetAccentClass(string name)
    {
        var normalizedName = NormalizeName(name);

        if (normalizedName.Contains("exam")) return "calendar-item-exam";
        if (normalizedName.Contains("quiz")) return "calendar-item-quiz";
        if (normalizedName.Contains("test")) return "calendar-item-test";
        if (normalizedName.Contains("assignment")) return "calendar-item-assignment";
        if (normalizedName.Contains("project")) return "calendar-item-project";

        return "calendar-item-assessment";
    }

    private static string NormalizeName(string name)
    {
        return (name ?? string.Empty).Trim().ToLowerInvariant();
    }
}
