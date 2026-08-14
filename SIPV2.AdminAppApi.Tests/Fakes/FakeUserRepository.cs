using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Fakes;

public class FakeUserRepository : IUserRepository
{
    public List<Mduser> Users { get; } = [];

    public Task<List<Mduser>> GetAllAsync() => Task.FromResult(Users.ToList());

    public Task<Mduser?> GetByIdAsync(Guid id) => Task.FromResult(Users.FirstOrDefault(u => u.Id == id));

    public Task<Mduser?> GetByLoginAsync(string login) => Task.FromResult(Users.FirstOrDefault(u => u.Login == login));

    public Task<Mduser> CreateAsync(Mduser user)
    {
        user.Id = Guid.NewGuid();
        Users.Add(user);
        return Task.FromResult(user);
    }

    public Task<bool> UpdateAsync(Mduser user)
    {
        var existing = Users.FirstOrDefault(u => u.Id == user.Id);
        if (existing is null)
        {
            return Task.FromResult(false);
        }

        existing.Name = user.Name;
        existing.LastName = user.LastName;
        existing.LastName1 = user.LastName1;
        existing.Email = user.Email;
        existing.Phone = user.Phone;
        existing.Active = user.Active;
        return Task.FromResult(true);
    }

    public Task<bool> UpdatePasswordAsync(Guid id, string passwordHash)
    {
        var existing = Users.FirstOrDefault(u => u.Id == id);
        if (existing is null)
        {
            return Task.FromResult(false);
        }

        existing.Password = passwordHash;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var existing = Users.FirstOrDefault(u => u.Id == id);
        if (existing is null)
        {
            return Task.FromResult(false);
        }

        Users.Remove(existing);
        return Task.FromResult(true);
    }
}
