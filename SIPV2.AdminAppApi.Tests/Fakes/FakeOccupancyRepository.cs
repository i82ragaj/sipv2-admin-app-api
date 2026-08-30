using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Fakes;

public class FakeOccupancyRepository : IOccupancyRepository
{
    public List<VoccupationActual> Occupancy { get; } = [];

    public Task<List<VoccupationActual>> GetCurrentAsync(string? parkingId, string? counterName)
    {
        // Solo contadores cuyo nombre contiene "Todos" (ver EfOccupancyRepository).
        var query = Occupancy.AsEnumerable()
            .Where(o => o.CounterName != null && o.CounterName.Contains("Todos", StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(parkingId))
        {
            query = query.Where(o => o.ParkingId == parkingId);
        }

        if (!string.IsNullOrWhiteSpace(counterName))
        {
            query = query.Where(o =>
                o.CounterName != null && o.CounterName.Contains(counterName, StringComparison.OrdinalIgnoreCase));
        }

        return Task.FromResult(query.OrderBy(o => o.ParkingId).ThenBy(o => o.CounterCode).ToList());
    }
}
