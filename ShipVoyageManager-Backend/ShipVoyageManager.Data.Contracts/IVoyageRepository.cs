using ShipVoyageManager.Data.Contracts.Helpers.DTO.Voyage;
using ShipVoyageManager.Data.Object;

namespace ShipVoyageManager.Data.Contracts;
public interface IVoyageRepository
{
    public Task<PaginatedVoyagesDto> GetVoyagesPaginatedAsync(int page, int pageSize);
    public Task AddVoyageAsync(VoyageEntity voyage);
    public Task UpdateVoyageAsync(VoyageEntity voyage);
    public Task DeleteVoyageByIdAsync(Guid id);
}
