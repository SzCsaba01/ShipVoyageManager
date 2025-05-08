using ShipVoyageManager.Data.Contracts.Helpers.DTO.Ship;

namespace ShipVoyageManager.Service.Contracts;
public interface IShipService
{
    public Task<FilteredPaginatedShipsDto> GetFilteredShipsPaginatedAsync(int page, int pageSize, string search);
    public Task<List<ShipDto>> GetShipsOutOfDateRangeAsync(DateTime startDate, DateTime endDate);
    public Task AddShipAsync(ShipDto shipDto);
    public Task UpdateShipAsync(ShipDto shipDto);
    public Task DeleteShipByIdAsync(Guid id);
}
