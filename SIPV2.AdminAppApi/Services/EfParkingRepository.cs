using Microsoft.EntityFrameworkCore;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

public class EfParkingRepository : IParkingRepository
{
    private readonly AppDbContext _context;

    public EfParkingRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<Mdparking>> GetAllAsync() =>
        _context.Mdparkings.AsNoTracking().ToListAsync();

    public Task<Mdparking?> GetByIdAsync(string id) =>
        _context.Mdparkings.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Mdparking> CreateAsync(Mdparking parking)
    {
        parking.Active = true;
        parking.Created = DateTime.UtcNow;
        _context.Mdparkings.Add(parking);

        await EnsureParkingStatusAsync(parking.Id);

        await _context.SaveChangesAsync();
        return parking;
    }

    public async Task<bool> UpdateAsync(Mdparking parking)
    {
        var existing = await _context.Mdparkings.FirstOrDefaultAsync(p => p.Id == parking.Id);
        if (existing is null)
        {
            return false;
        }

        existing.Active = parking.Active;
        existing.Type = parking.Type;
        existing.Srv = parking.Srv;
        existing.Name = parking.Name;
        existing.Company = parking.Company;
        existing.Dacode = parking.Dacode;
        existing.DateFromTable = parking.DateFromTable;
        existing.DateToTable = parking.DateToTable;
        existing.Ndays = parking.Ndays;
        existing.TruncateTables = parking.TruncateTables;
        existing.Sii = parking.Sii;
        existing.MultiCounter = parking.MultiCounter;
        existing.ServerIp = parking.ServerIp;
        existing.Job = parking.Job;
        existing.LoadDate = parking.LoadDate;
        existing.Frecuency = parking.Frecuency;
        existing.Updated = DateTime.UtcNow;

        await EnsureParkingStatusAsync(existing.Id);

        await _context.SaveChangesAsync();
        return true;
    }

    // "Eliminar" es borrado lógico en toda la aplicación: se desactiva el
    // registro (Active = false) en vez de borrar la fila físicamente.
    public async Task<bool> DeleteAsync(string id)
    {
        var existing = await _context.Mdparkings.FirstOrDefaultAsync(p => p.Id == id);
        if (existing is null)
        {
            return false;
        }

        existing.Active = false;
        existing.Updated = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    // MDParkingStatus lo alimenta normalmente el proceso de importación
    // (ver ParkingStatusesController), pero un parking recién creado -o uno
    // legacy que nunca llegó a importar- puede no tener fila todavía. Al
    // crear/editar un parking nos aseguramos de que exista, con valores por
    // defecto, para que "Estado de parkings" no lo deje fuera.
    private async Task EnsureParkingStatusAsync(string parkingId)
    {
        var exists = await _context.MdparkingStatuses.AnyAsync(s => s.Id == parkingId);
        if (exists)
        {
            return;
        }

        _context.MdparkingStatuses.Add(new MdparkingStatus
        {
            Id = parkingId,
            Active = true,
            Created = DateTime.UtcNow,
        });
    }
}
