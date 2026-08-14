using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

public interface IUserRepository
{
    Task<List<Mduser>> GetAllAsync();

    Task<Mduser?> GetByIdAsync(Guid id);

    Task<Mduser?> GetByLoginAsync(string login);

    Task<Mduser> CreateAsync(Mduser user);

    Task<bool> UpdateAsync(Mduser user);

    // passwordHash ya debe venir hasheado (BCrypt); el repositorio no hashea.
    Task<bool> UpdatePasswordAsync(Guid id, string passwordHash);

    Task<bool> DeleteAsync(Guid id);
}
