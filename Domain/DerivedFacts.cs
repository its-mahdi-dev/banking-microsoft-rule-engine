namespace BankDecisionApi.Domain;

public class DerivedFacts
{
    public decimal DailyAmount { get; set; }
    public int DailyCount { get; set; }
    public decimal WeeklyAmount { get; set; }
    public int WeeklyCount { get; set; }
    public decimal MonthlyAmount { get; set; }
    public int MonthlyCount { get; set; }
    public int DeviceVelocityLastHour { get; set; }
    public double RealTimeRiskScore { get; set; } // 0..1
}
