namespace AssessmentTrackingSystem.Services;

public static class AvatarUrlBuilder
{
    public static string Build(string? seed)
    {
        var safeSeed = string.IsNullOrWhiteSpace(seed)
            ? "student"
            : Uri.EscapeDataString(seed.Trim());

        return $"https://api.dicebear.com/9.x/fun-emoji/svg?seed={safeSeed}";
    }
}
