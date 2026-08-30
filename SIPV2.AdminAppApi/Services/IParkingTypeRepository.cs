using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

// MDParkingType es el catálogo de tipos de parking (antes una constante fija
// de 3 valores en el front). Solo lectura desde esta API: se mantiene desde
// base de datos, no hay pantalla de alta/edición todavía.
public interface IParkingTypeRepository
{
    Task<List<MdparkingType>> GetAllAsync();
}
