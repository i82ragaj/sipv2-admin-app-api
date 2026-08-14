using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Fakes;

public class FakeParkingStatusRepository : IParkingStatusRepository
{
    public List<MdparkingStatus> Statuses { get; } = [];

    public Task<List<MdparkingStatus>> GetAllAsync() => Task.FromResult(Statuses.ToList());

    public Task<MdparkingStatus?> GetByIdAsync(string id) => Task.FromResult(Statuses.FirstOrDefault(s => s.Id == id));
}
