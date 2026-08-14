using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

public interface IRolRepository
{
    Task<List<Mdrol>> GetAllAsync();

    Task<Mdrol?> GetByIdAsync(Guid id);

    Task<Mdrol> CreateAsync(Mdrol rol);

    Task<bool> UpdateAsync(Mdrol rol);

    Task<bool> DeleteAsync(Guid id);
}
