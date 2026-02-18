namespace ATS.Core.Entities;

public class Course
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;  // Will connect to Identity later

    public List<Assessment> Assessments { get; set; } = new();
}