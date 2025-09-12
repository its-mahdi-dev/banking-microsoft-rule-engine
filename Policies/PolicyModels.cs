namespace BankDecisionApi.Policies;

public class CurrencyLimits
{
    public string Currency { get; set; } = "USD";
    public decimal DailyMax { get; set; }
    public decimal WeeklyMax { get; set; }
    public decimal MonthlyMax { get; set; }
}

public class InternationalPolicy
{
    public decimal NoKycMax { get; set; }
    public HashSet<string> HighRiskCountries { get; set; } = new();
    public HashSet<string> HighRiskBanks { get; set; } = new();
}

public class MccPolicy
{
    public string Mcc { get; set; } = default!;
    public decimal ReviewThreshold { get; set; }
    public bool IsHighRisk { get; set; }
}

public class FeePolicy
{
    public string Currency { get; set; } = "USD";
    public decimal BaseFee { get; set; }
    public decimal VipDiscountPercent { get; set; } // 0..100
    public Dictionary<string, decimal> ChannelExtraPercent { get; set; } = new(); // e.g. WEB:0, APP:-10
}

public class PolicyContext
{
    public Dictionary<string, CurrencyLimits> LimitsByCurrency { get; set; } = new();
    public InternationalPolicy International { get; set; } = new();
    public Dictionary<string, MccPolicy> MccPolicies { get; set; } = new();
    public HashSet<string> RedHoursUtc { get; set; } = new(); // "00","01",...
    public HashSet<DateOnly> Holidays { get; set; } = new();
    public Dictionary<string, FeePolicy> FeeByCurrency { get; set; } = new();
}
