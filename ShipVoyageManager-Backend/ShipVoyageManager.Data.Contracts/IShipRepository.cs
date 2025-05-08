using ShipVoyageManager.Data.Contracts.Helpers.DTO.Ship;
using ShipVoyageManager.Data.Object;

namespace ShipVoyageManager.Data.Contracts;
public interface IShipRepository
{
    public Task<FilteredPaginatedShipsDto> GetFilteredShipsPaginatedAsync(int page, int pageSize, string search);
    public Task<ShipEntity?> GetShipByNameAsync(string name);
    public Task<List<ShipEntity>> GetShipsOutOfDateRangeAsync(DateTime startDate, DateTime endDate);
    public Task AddShipAsync(ShipEntity ship);
    public Task UpdateShipAsync(ShipEntity ship);
    public Task DeleteShipByIdAsync(Guid id);
}
