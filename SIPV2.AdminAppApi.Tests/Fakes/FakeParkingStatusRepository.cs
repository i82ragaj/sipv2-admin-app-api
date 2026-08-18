using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Fakes;

public class FakeParkingStatusRepository : IParkingStatusRepository
{
    public List<MdparkingStatus> Statuses { get; } = [];

    public Task<List<MdparkingStatus>> GetAllAsync() => Task.FromResult(Statuses.ToList());

    public Task<MdparkingStatus?> GetByIdAsync(string id) => Task.FromResult(Statuses.FirstOrDefault(s => s.Id == id));

    public Task<RequestDailyImportResult> RequestDailyImportAsync(string id)
    {
        var existing = Statuses.FirstOrDefault(s => s.Id == id);
        if (existing is null)
        {
            return Task.FromResult(RequestDailyImportResult.NotFound);
        }

        if (existing.LastImportedStatus is not (null or "OK" or "ERROR"))
        {
            return Task.FromResult(RequestDailyImportResult.NotAllowed);
        }

        existing.LastImportedStatus = "PENDIENTE";
        return Task.FromResult(RequestDailyImportResult.Success);
    }
}
