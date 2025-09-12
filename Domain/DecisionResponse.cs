namespace BankDecisionApi.Domain;

public class DecisionResponse
{
    public Decision Decision { get; set; }
    public List<string> Reasons { get; set; } = new();
    public decimal? AppliedFee { get; set; }
}
