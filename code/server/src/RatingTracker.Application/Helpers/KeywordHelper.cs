namespace RatingTracker.Application.Helpers;

public static class KeywordHelper
{
    public static string KeywordsCleanup(string rawKeywords)
    {
        return string.Join(",", rawKeywords
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(k => k.Trim().Replace(" ", "")));
    }
}