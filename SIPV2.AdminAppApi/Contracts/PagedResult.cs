namespace SIPV2.AdminAppApi.Contracts;

// Envoltorio genérico para listados paginados en servidor.
public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
}
