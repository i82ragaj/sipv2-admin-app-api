using Microsoft.EntityFrameworkCore;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

public class EfOccupancyRepository : IOccupancyRepository
{
    private readonly AppDbContext _context;

    public EfOccupancyRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<VoccupationActual>> GetCurrentAsync(string? parkingId, string? counterName)
    {
        // Solo contadores cuyo nombre contiene "Todos" (sin distinguir
        // mayúsculas/minúsculas): son los contadores agregados del parking,
        // que es lo único relevante para este listado.
        var query = _context.VoccupationActuals
            .AsNoTracking()
            .Where(o => o.CounterName != null && o.CounterName.ToLower().Contains("todos"));

        if (!string.IsNullOrWhiteSpace(parkingId))
        {
            query = query.Where(o => o.ParkingId == parkingId);
        }

        if (!string.IsNullOrWhiteSpace(counterName))
        {
            query = query.Where(o => o.CounterName != null && o.CounterName.Contains(counterName));
        }

        return query.OrderBy(o => o.ParkingId).ThenBy(o => o.CounterCode).ToListAsync();
    }
}
