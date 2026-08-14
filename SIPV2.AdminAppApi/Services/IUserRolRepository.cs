using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

public interface IUserRolRepository
{
    Task<List<MduserRol>> GetAllAsync();

    Task<MduserRol?> GetByIdAsync(Guid id);

    Task<MduserRol?> GetByUserAndRolAsync(Guid userId, Guid rolId);

    Task<MduserRol> CreateAsync(MduserRol userRol);

    Task<bool> UpdateActiveAsync(Guid id, bool active);

    Task<bool> DeleteAsync(Guid id);
}
