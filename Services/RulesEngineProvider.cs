using RulesEngine.Models;

namespace BankDecisionApi.Services;

public class RulesEngineProvider : IRulesEngineProvider
{
    public RulesEngine.RulesEngine Engine { get; }

    public RulesEngineProvider(string jsonContent)
    {
        var workflows = System.Text.Json.JsonSerializer.Deserialize<List<Workflow>>(jsonContent,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        var settings = new ReSettings
        {
            CustomTypes = new[] { typeof(RuleUtils) }
        };

        Engine = new RulesEngine.RulesEngine(workflows.ToArray(), settings);
    }
}
