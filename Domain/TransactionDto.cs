namespace BankDecisionApi.Domain;

public class TransactionDto
{
    public string Id { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Type { get; set; } = "Domestic"; // Domestic|International|...
    public string CountryFrom { get; set; } = "AZ";
    public string CountryTo { get; set; } = "AZ";
    public string Mcc { get; set; } = "5999";
    public DateTime UtcTime { get; set; }
    public string DeviceId { get; set; } = default!;
    public string Channel { get; set; } = "WEB"; // WEB|APP|POS|...
    public string CardCountry { get; set; } = "AZ";
    public string IpCountry { get; set; } = "AZ";
}
