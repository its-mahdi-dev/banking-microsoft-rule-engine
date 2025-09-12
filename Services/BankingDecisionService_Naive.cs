using BankDecisionApi.Domain;
using BankDecisionApi.Policies;

namespace BankDecisionApi.Services;

public class BankingDecisionService_Naive : IBankingDecisionServiceNaive
{
    private readonly IPolicyRepository _repo;
    public BankingDecisionService_Naive(IPolicyRepository repo) => _repo = repo;

    public async Task<DecisionResponse> EvaluateAsync(TransactionDto txn, AccountDto acct, DerivedFacts facts, CancellationToken ct=default)
    {
        var ctx = await _repo.LoadAsync(ct);
        var reasons = new List<string>();
        var decision = Decision.Approve;

        if (acct.Suspended) return new DecisionResponse { Decision = Decision.Block, Reasons = { "AccountSuspended" } };
        if (acct.IsBlacklisted) return new DecisionResponse { Decision = Decision.Block, Reasons = { "AccountBlacklisted" } };

        if (acct.IsVip)
        {
            if (txn.Type == "International" && acct.KycLevel < 1)
                return new DecisionResponse { Decision = Decision.Block, Reasons = { "VipButNoKycIntl" } };

            reasons.Add("VipCandidateForAutoApprove");
        }

        if (txn.Type == "International")
        {
            var intl = ctx.International;
            if (acct.KycLevel < 2 && txn.Amount > intl.NoKycMax)
                return new DecisionResponse { Decision = Decision.Block, Reasons = { "IntlNoKycOverLimit" } };

            if (intl.HighRiskCountries.Contains(txn.CountryTo))
            {
                decision = Decision.Block; reasons.Add("HighRiskDestination");
            }
            if (intl.HighRiskCountries.Contains(txn.CountryFrom) && txn.Currency != "USD")
            {
                decision = Decision.Block; reasons.Add("HighRiskOriginNonUSD");
            }
        }

        if (ctx.LimitsByCurrency.TryGetValue(txn.Currency, out var lim))
        {
            if (facts.DailyAmount + txn.Amount > lim.DailyMax)
            {
                decision = Max(decision, Decision.Review); reasons.Add("DailyLimitExceeded");
            }
            if (facts.WeeklyAmount + txn.Amount > lim.WeeklyMax)
            {
                decision = Max(decision, Decision.Review); reasons.Add("WeeklyLimitExceeded");
            }
            if (facts.MonthlyAmount + txn.Amount > lim.MonthlyMax)
            {
                decision = Decision.Block; reasons.Add("MonthlyLimitExceeded");
            }
        }

        if (ctx.MccPolicies.TryGetValue(txn.Mcc, out var mccPol))
        {
            if (mccPol.IsHighRisk && txn.Amount > mccPol.ReviewThreshold)
            {
                decision = Max(decision, Decision.Review); reasons.Add($"MccHighRiskOverThresh:{txn.Mcc}");
            }
        }

        var hourKey = txn.UtcTime.Hour.ToString("00");
        var isRedHour = ctx.RedHoursUtc.Contains(hourKey);
        if (!string.Equals(txn.IpCountry, txn.CardCountry, StringComparison.OrdinalIgnoreCase) && isRedHour)
        {
            decision = Max(decision, Decision.Review); reasons.Add("GeoMismatchAtRedHour");
        }
        if (facts.DeviceVelocityLastHour > 50)
        {
            decision = Max(decision, Decision.Review); reasons.Add("DeviceVelocityHigh");
        }
        if (ctx.International.HighRiskCountries.Contains(txn.CountryTo)
            && txn.Type == "International"
            && ctx.Holidays.Contains(DateOnly.FromDateTime(txn.UtcTime)))
        {
            decision = Decision.Block; reasons.Add("IntlHighRiskOnHoliday");
        }

        switch (acct.ChargebackCountLast90d)
        {
            case >= 5:
                decision = Decision.Block; reasons.Add("TooManyChargebacks"); break;
            case >= 2:
                decision = Max(decision, Decision.Review); reasons.Add("ChargebacksWarning"); break;
        }

        if (facts.RealTimeRiskScore >= 0.9) { decision = Decision.Block; reasons.Add("VeryHighRiskScore"); }
        else if (facts.RealTimeRiskScore >= 0.7) { decision = Max(decision, Decision.Review); reasons.Add("HighRiskScore"); }

        decimal? fee = null;
        if (decision != Decision.Block && ctx.FeeByCurrency.TryGetValue(txn.Currency, out var fp))
        {
            var baseFee = fp.BaseFee;
            var chanExtraPct = fp.ChannelExtraPercent.TryGetValue(txn.Channel, out var extra) ? extra : 0m;
            var vipDisc = acct.IsVip ? fp.VipDiscountPercent : 0m;
            fee = baseFee * (1 + (chanExtraPct/100m)) * (1 - (vipDisc/100m));
        }

        return new DecisionResponse { Decision = decision, Reasons = reasons, AppliedFee = fee };

        static Decision Max(Decision a, Decision b) => (Decision)Math.Max((int)a, (int)b);
    }
}
