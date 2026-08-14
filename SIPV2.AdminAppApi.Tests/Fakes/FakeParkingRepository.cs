using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Fakes;

public class FakeParkingRepository : IParkingRepository
{
    public List<Mdparking> Parkings { get; } = [];

    public Task<List<Mdparking>> GetAllAsync() => Task.FromResult(Parkings.ToList());

    public Task<Mdparking?> GetByIdAsync(string id) => Task.FromResult(Parkings.FirstOrDefault(p => p.Id == id));

    public Task<Mdparking> CreateAsync(Mdparking parking)
    {
        parking.Active = true;
        Parkings.Add(parking);
        return Task.FromResult(parking);
    }

    public Task<bool> UpdateAsync(Mdparking parking)
    {
        var existing = Parkings.FirstOrDefault(p => p.Id == parking.Id);
        if (existing is null)
        {
            return Task.FromResult(false);
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
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(string id)
    {
        var existing = Parkings.FirstOrDefault(p => p.Id == id);
        if (existing is null)
        {
            return Task.FromResult(false);
        }

        Parkings.Remove(existing);
        return Task.FromResult(true);
    }
}
