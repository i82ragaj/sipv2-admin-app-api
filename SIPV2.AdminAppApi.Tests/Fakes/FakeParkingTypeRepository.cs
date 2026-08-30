using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Fakes;

public class FakeParkingTypeRepository : IParkingTypeRepository
{
    public List<MdparkingType> Types { get; } = [];

    public Task<List<MdparkingType>> GetAllAsync() =>
        Task.FromResult(Types.OrderBy(t => t.Name).ToList());
}
