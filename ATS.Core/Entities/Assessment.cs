namespace ATS.Core.Entities;

public class Assessment
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime DueDate { get; set; }

    public string Status { get; set; } = "Not Started";

    public int CourseId { get; set; }

    public Course? Course { get; set; }
}