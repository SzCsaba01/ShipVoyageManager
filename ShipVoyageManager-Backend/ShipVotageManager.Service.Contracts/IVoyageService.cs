using ShipVoyageManager.Data.Contracts.Helpers.DTO.Port;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.Voyage;

namespace ShipVoyageManager.Service.Contracts;
public interface IVoyageService
{
    public Task<PaginatedVoyagesDto> GetVoyagesPaginatedAsync(int page, int pageSize);
    public Task AddVoyageAsync(VoyageDto voyageDto);
    public Task UpdateVoyageAsync(VoyageDto voyageDto);
    public Task DeleteVoyageAsync(Guid id);
}
