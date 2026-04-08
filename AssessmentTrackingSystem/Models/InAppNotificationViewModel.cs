namespace AssessmentTrackingSystem.Models;

public class InAppNotificationViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<string> Details { get; set; } = new();
    public string Type { get; set; } = string.Empty;
    public string IconClass { get; set; } = "bi bi-bell";
    public string ActionText { get; set; } = string.Empty;
    public string ActionUrl { get; set; } = string.Empty;
}
