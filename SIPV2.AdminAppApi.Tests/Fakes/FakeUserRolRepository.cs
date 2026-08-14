using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Fakes;

public class FakeUserRolRepository : IUserRolRepository
{
    public List<MduserRol> UserRoles { get; } = [];

    public Task<List<MduserRol>> GetAllAsync() => Task.FromResult(UserRoles.ToList());

    public Task<MduserRol?> GetByIdAsync(Guid id) => Task.FromResult(UserRoles.FirstOrDefault(ur => ur.Id == id));

    public Task<MduserRol?> GetByUserAndRolAsync(Guid userId, Guid rolId) =>
        Task.FromResult(UserRoles.FirstOrDefault(ur => ur.UserId == userId && ur.RolId == rolId));

    public Task<MduserRol> CreateAsync(MduserRol userRol)
    {
        userRol.Id = Guid.NewGuid();
        userRol.Active = true;
        UserRoles.Add(userRol);
        return Task.FromResult(userRol);
    }

    public Task<bool> UpdateActiveAsync(Guid id, bool active)
    {
        var existing = UserRoles.FirstOrDefault(ur => ur.Id == id);
        if (existing is null)
        {
            return Task.FromResult(false);
        }

        existing.Active = active;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var existing = UserRoles.FirstOrDefault(ur => ur.Id == id);
        if (existing is null)
        {
            return Task.FromResult(false);
        }

        UserRoles.Remove(existing);
        return Task.FromResult(true);
    }
}
