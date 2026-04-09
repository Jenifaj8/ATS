namespace AssessmentTrackingSystem.Services;

public static class AvatarUrlBuilder
{
    private const string BaseUrl = "https://api.dicebear.com/9.x/thumbs/svg";

    public static string Build(string? seed)
    {
        var normalizedSeed = string.IsNullOrWhiteSpace(seed)
            ? "study-buddy"
            : seed.Trim();

        return $"{BaseUrl}?seed={Uri.EscapeDataString(normalizedSeed)}&backgroundType=gradientLinear";
    }
}
