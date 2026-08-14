using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Fakes;

public class FakeRolRepository : IRolRepository
{
    public List<Mdrol> Roles { get; } = [];

    public Task<List<Mdrol>> GetAllAsync() => Task.FromResult(Roles.ToList());

    public Task<Mdrol?> GetByIdAsync(Guid id) => Task.FromResult(Roles.FirstOrDefault(r => r.Id == id));

    public Task<Mdrol> CreateAsync(Mdrol rol)
    {
        rol.Id = Guid.NewGuid();
        rol.Active = true;
        Roles.Add(rol);
        return Task.FromResult(rol);
    }

    public Task<bool> UpdateAsync(Mdrol rol)
    {
        var existing = Roles.FirstOrDefault(r => r.Id == rol.Id);
        if (existing is null)
        {
            return Task.FromResult(false);
        }

        existing.Name = rol.Name;
        existing.Description = rol.Description;
        existing.Active = rol.Active;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var existing = Roles.FirstOrDefault(r => r.Id == id);
        if (existing is null)
        {
            return Task.FromResult(false);
        }

        Roles.Remove(existing);
        return Task.FromResult(true);
    }
}
