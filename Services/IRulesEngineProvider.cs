using RulesEngine;

namespace BankDecisionApi.Services;

public interface IRulesEngineProvider
{
    RulesEngine.RulesEngine Engine { get; }
}
