using ShipVoyageManager.Data.Contracts.Helpers.DTO.Port;
using ShipVoyageManager.Data.Object;

namespace ShipVoyageManager.Data.Contracts;
public interface IPortRepository
{
    public Task<FilteredPaginatedPortsDto> GetFilteredPortsPaginatedAsync(int page, int pageSize, string search);
    public Task<PortEntity?> GetPortByNameAsync(string name);
    public Task<List<PortEntity>> GetAllPortsAsync();
    public Task AddPortAsync(PortEntity port);
    public Task UpdatePortAsync(PortEntity port);
    public Task DeletePortByIdAsync(Guid id);
}
