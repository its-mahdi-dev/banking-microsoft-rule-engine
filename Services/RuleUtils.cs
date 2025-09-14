namespace BankDecisionApi.Services;

public static class RuleUtils
{
    public static bool In<T>(T value, params T[] set) => set.Contains(value);
    public static bool Over(decimal lhs, decimal rhs) => lhs > rhs;
    public static bool IsRedHour(DateTime utc, HashSet<string> redHours) =>
        redHours.Contains(utc.Hour.ToString("00"));
    public static bool IsHoliday(DateTime utc, HashSet<DateOnly> holidays) =>
        holidays.Contains(DateOnly.FromDateTime(utc));
    public static bool GeoMismatch(string ipC, string cardC) =>
        !string.Equals(ipC, cardC, StringComparison.OrdinalIgnoreCase);

    public static bool EvaluateDeviceTrust(
        bool baselineRisk,
        int deviceVelocityLastHour,
        double riskScore,
        bool isVip,
        bool isRedHour)
    {
        if (isVip && !baselineRisk && riskScore < 0.8) return true;

        var velocityCap = isRedHour ? 30 : 50;
        if (deviceVelocityLastHour > velocityCap) return false;

        if (baselineRisk && riskScore >= 0.7) return false;
        if (!baselineRisk && riskScore <= 0.6) return true;

        return riskScore < 0.8;
    }
}
