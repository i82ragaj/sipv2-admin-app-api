namespace SIPV2.AdminAppApi.Contracts.DailyTotals;

public class DailyTotalDto
{
    public string Idpk { get; set; } = null!;

    public DateTime TotalDate { get; set; }

    public decimal? DailyTotalAmount { get; set; }

    public int? DailyTransWithPay { get; set; }

    public int? DailyTransWithOutPay { get; set; }
}
