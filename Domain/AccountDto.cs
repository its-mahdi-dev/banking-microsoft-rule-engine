namespace BankDecisionApi.Domain;

public class AccountDto
{
    public string AccountId { get; set; } = default!;
    public bool IsVip { get; set; }
    public int KycLevel { get; set; } // 0..3
    public bool IsBlacklisted { get; set; }
    public bool Suspended { get; set; }
    public int ChargebackCountLast90d { get; set; }
    public string ResidenceCountry { get; set; } = "AZ";
}
