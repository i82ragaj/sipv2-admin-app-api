namespace SIPV2.AdminAppApi.Contracts.ParkingSummaries;

public class ParkingSummaryDto
{
    public string? Id { get; set; }

    public string Idpk { get; set; } = null!;

    public DateOnly? Date { get; set; }

    public int? Operations { get; set; }

    public decimal? Total { get; set; }

    public string? InvoiceNoMin { get; set; }

    public string? InvoiceNoMax { get; set; }

    public decimal? ParkingTransTotal { get; set; }

    public int? ParkingTransNum { get; set; }

    public int? SumMinutes { get; set; }

    public decimal? InvoicesTotal { get; set; }

    public int? InvoicesNum { get; set; }

    public decimal? Acatotal { get; set; }

    public int? Acanum { get; set; }

    public decimal? Ceitotal { get; set; }

    public int? Ceinum { get; set; }

    public decimal? Certotal { get; set; }

    public int? Cernum { get; set; }

    public decimal? CashTotal { get; set; }

    public int? CashNum { get; set; }

    public decimal? RestTotal { get; set; }

    public int? RestNum { get; set; }
}
