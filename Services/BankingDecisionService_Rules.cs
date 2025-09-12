using BankDecisionApi.Domain;
using BankDecisionApi.Policies;
using RulesEngine.Models;

namespace BankDecisionApi.Services;

public class BankingDecisionService_Rules : IBankingDecisionServiceRules
{
    private readonly IPolicyRepository _repo;
    private readonly IRulesEngineProvider _prov;

    public BankingDecisionService_Rules(IPolicyRepository repo, IRulesEngineProvider prov)
    {
        _repo = repo; _prov = prov;
    }

    public async Task<DecisionResponse> EvaluateAsync(TransactionDto txn, AccountDto acct, DerivedFacts facts, CancellationToken ct=default)
    {
        var ctx = await _repo.LoadAsync(ct);

        var pTxn   = new RuleParameter("txn", txn);
        var pAcct  = new RuleParameter("acct", acct);
        var pFacts = new RuleParameter("facts", facts);
        var pCtx   = new RuleParameter("ctx", ctx);

        var results = await _prov.Engine.ExecuteAllRulesAsync("TransactionRules", pTxn, pAcct, pFacts, pCtx);

        var reasons = new List<string>();
        var decision = Decision.Approve;

        foreach (var r in results)
        {
            if (!r.IsSuccess) continue;
            var ev = r.Rule.SuccessEvent?.Trim();
            if (string.IsNullOrWhiteSpace(ev)) continue;

            reasons.Add(ev);

            if (ev.StartsWith("BLOCK:", StringComparison.OrdinalIgnoreCase))
                decision = Decision.Block;
            else if (ev.StartsWith("REVIEW:", StringComparison.OrdinalIgnoreCase) && decision != Decision.Block)
                decision = Decision.Review;
            // APPROVE:* تاثیر ندارد اگر قبلا Review/Block شده
        }

        // Fee if not blocked
        decimal? fee = null;
        if (decision != Decision.Block && ctx.FeeByCurrency.TryGetValue(txn.Currency, out var fp))
        {
            var baseFee = fp.BaseFee;
            var chanExtraPct = fp.ChannelExtraPercent.TryGetValue(txn.Channel, out var extra) ? extra : 0m;
            var vipDisc = acct.IsVip ? fp.VipDiscountPercent : 0m;
            fee = baseFee * (1 + (chanExtraPct/100m)) * (1 - (vipDisc/100m));
        }

        return new DecisionResponse { Decision = decision, Reasons = reasons, AppliedFee = fee };
    }
}
