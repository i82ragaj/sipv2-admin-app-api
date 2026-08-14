using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

public interface IAuthRepository
{
    Task<Mduser?> GetActiveUserByLoginAsync(string login);

    Task<List<string>> GetRoleNamesAsync(Guid userId);
}
