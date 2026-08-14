using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Fakes;

public class FakeAuthRepository : IAuthRepository
{
    public List<Mduser> Users { get; } = [];

    public List<MduserRol> UserRoles { get; } = [];

    public List<Mdrol> Roles { get; } = [];

    public Task<Mduser?> GetActiveUserByLoginAsync(string login) =>
        Task.FromResult(Users.FirstOrDefault(u => u.Login == login && u.Active));

    public Task<List<string>> GetRoleNamesAsync(Guid userId)
    {
        var roleIds = UserRoles.Where(ur => ur.UserId == userId && ur.Active).Select(ur => ur.RolId);
        var names = Roles.Where(r => roleIds.Contains(r.Id) && r.Active).Select(r => r.Name!).ToList();
        return Task.FromResult(names);
    }
}
