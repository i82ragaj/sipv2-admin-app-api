using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Fakes;

public class FakeJwtTokenService : IJwtTokenService
{
    public string GenerateToken(Mduser user, IEnumerable<string> roles) => $"fake-token-{user.Id}";
}
