namespace SIPV2.AdminAppApi.Contracts.ParkingSummaries;

// Refleja VParkingSummaryDetail: desglose por tipo de pago de una fila de
// VParkingSummary (Idsummary -> ParkingSummaryDto.Id).
public class ParkingSummaryDetailDto
{
    public string? PaymentTypeId { get; set; }
    public string? PaymentTypeName { get; set; }
    public int? Operations { get; set; }
    public decimal? Total { get; set; }
    public int? Discount { get; set; }
}
