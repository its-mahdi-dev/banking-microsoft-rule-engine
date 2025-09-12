namespace BankDecisionApi.Policies;

public class InMemoryPolicyRepository : IPolicyRepository
{
    private PolicyContext? _cache;

    public Task<PolicyContext> LoadAsync(CancellationToken ct = default)
    {
        if (_cache != null) return Task.FromResult(_cache);

        var ctx = new PolicyContext
        {
            LimitsByCurrency = new Dictionary<string, CurrencyLimits>
            {
                ["USD"] = new CurrencyLimits { Currency="USD", DailyMax=3000, WeeklyMax=15000, MonthlyMax=50000 },
                ["EUR"] = new CurrencyLimits { Currency="EUR", DailyMax=2500, WeeklyMax=12000, MonthlyMax=40000 },
                ["AZN"] = new CurrencyLimits { Currency="AZN", DailyMax=10000, WeeklyMax=50000, MonthlyMax=150000 }
            },
            International = new InternationalPolicy
            {
                NoKycMax = 1000,
                HighRiskCountries = new HashSet<string> { "IR","KP","SY","AF" },
                HighRiskBanks = new HashSet<string> { "HRB001","HRB777" }
            },
            MccPolicies = new Dictionary<string, MccPolicy>
            {
                ["4829"] = new MccPolicy { Mcc="4829", IsHighRisk=true, ReviewThreshold=2000 }, // money transfer
                ["7995"] = new MccPolicy { Mcc="7995", IsHighRisk=true, ReviewThreshold=500  }, // gambling
                ["6012"] = new MccPolicy { Mcc="6012", IsHighRisk=true, ReviewThreshold=1000 }  // financial inst
            },
            RedHoursUtc = new HashSet<string> { "00","01","02","03","23" },
            Holidays = new HashSet<DateOnly> { DateOnly.FromDateTime(DateTime.UtcNow.Date) },
            FeeByCurrency = new Dictionary<string, FeePolicy>
            {
                ["USD"] = new FeePolicy { Currency="USD", BaseFee=1.50m, VipDiscountPercent=50, ChannelExtraPercent={ ["WEB"]=0, ["APP"]=-10, ["POS"]=5 } },
                ["EUR"] = new FeePolicy { Currency="EUR", BaseFee=1.40m, VipDiscountPercent=40, ChannelExtraPercent={ ["WEB"]=0, ["APP"]=-5, ["POS"]=7 } },
            }
        };

        _cache = ctx;
        return Task.FromResult(ctx);
    }
}
