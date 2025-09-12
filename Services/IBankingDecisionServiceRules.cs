using BankDecisionApi.Domain;

namespace BankDecisionApi.Services;

public interface IBankingDecisionServiceRules
{
    Task<DecisionResponse> EvaluateAsync(TransactionDto txn, AccountDto acct, DerivedFacts facts, CancellationToken ct=default);
}
