using System.Threading;

namespace BankDecisionApi.Policies;

public interface IPolicyRepository
{
    Task<PolicyContext> LoadAsync(CancellationToken ct = default);
}
