using Microsoft.EntityFrameworkCore;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

public class EfParkingStatusRepository : IParkingStatusRepository
{
    private readonly AppDbContext _context;

    public EfParkingStatusRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<MdparkingStatus>> GetAllAsync() =>
        _context.MdparkingStatuses.AsNoTracking().Include(s => s.Parking).ToListAsync();

    public Task<MdparkingStatus?> GetByIdAsync(string id) =>
        _context.MdparkingStatuses.AsNoTracking().Include(s => s.Parking).FirstOrDefaultAsync(s => s.Id == id);

    private const string StatusOk = "OK";
    private const string StatusError = "ERROR";
    private const string StatusPending = "PENDIENTE";

    public async Task<RequestDailyImportResult> RequestDailyImportAsync(string id)
    {
        var existing = await _context.MdparkingStatuses.FirstOrDefaultAsync(s => s.Id == id);
        if (existing is null)
        {
            return RequestDailyImportResult.NotFound;
        }

        // Permitido cuando aún no se ha importado nunca (null) o cuando la
        // última importación terminó (OK/ERROR); no si ya está PENDIENTE.
        if (existing.LastImportedStatus is not (null or StatusOk or StatusError))
        {
            return RequestDailyImportResult.NotAllowed;
        }

        existing.LastImportedStatus = StatusPending;
        existing.Updated = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return RequestDailyImportResult.Success;
    }
}
