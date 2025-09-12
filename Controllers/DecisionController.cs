using BankDecisionApi.Domain;
using BankDecisionApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankDecisionApi.Controllers;

[ApiController]
[Route("api/decision")]
public class DecisionController : ControllerBase
{
    public class EvaluateRequest
    {
        public TransactionDto Transaction { get; set; } = default!;
        public AccountDto Account { get; set; } = default!;
        public DerivedFacts Facts { get; set; } = default!;
    }

    private readonly IBankingDecisionServiceNaive _naive;
    private readonly IBankingDecisionServiceRules _rules;

    public DecisionController(IBankingDecisionServiceNaive naive, IBankingDecisionServiceRules rules)
    {
        _naive = naive; _rules = rules;
    }

    [HttpPost("evaluate-naive")]
    public async Task<ActionResult<DecisionResponse>> EvaluateNaive([FromBody] EvaluateRequest req, CancellationToken ct)
        => Ok(await _naive.EvaluateAsync(req.Transaction, req.Account, req.Facts, ct));

    [HttpPost("evaluate-rules")]
    public async Task<ActionResult<DecisionResponse>> EvaluateRules([FromBody] EvaluateRequest req, CancellationToken ct)
        => Ok(await _rules.EvaluateAsync(req.Transaction, req.Account, req.Facts, ct));
}
