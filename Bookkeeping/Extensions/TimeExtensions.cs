namespace Bookkeeping.Extensions;

internal static class TimeExtensions
{
    public static IEnumerable<string> GetTimes() => Enumerable.Range(8, 12).Select(num => $"{num}-{num + 1}");
}