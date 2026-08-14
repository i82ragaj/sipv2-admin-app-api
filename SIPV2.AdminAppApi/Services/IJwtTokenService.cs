using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

public interface IJwtTokenService
{
    string GenerateToken(Mduser user, IEnumerable<string> roles);
}
