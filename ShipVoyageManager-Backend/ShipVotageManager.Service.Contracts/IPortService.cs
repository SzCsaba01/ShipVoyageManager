using ShipVoyageManager.Data.Contracts.Helpers.DTO.Port;

namespace ShipVoyageManager.Service.Contracts;
public interface IPortService
{
    public Task<FilteredPaginatedPortsDto> GetFilteredPortsPaginatedAsync(int page, int pageSize, string search);
    public Task<List<PortDto>> GetAllPortsAsync();
    public Task AddPortAsync(PortDto portDto);
    public Task UpdatePortAsync(PortDto portDto);
    public Task DeletePortByIdAsync(Guid id);
}
