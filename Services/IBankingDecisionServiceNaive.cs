using BankDecisionApi.Domain;

namespace BankDecisionApi.Services;

public interface IBankingDecisionServiceNaive
{
    Task<DecisionResponse> EvaluateAsync(TransactionDto txn, AccountDto acct, DerivedFacts facts, CancellationToken ct=default);
}
