using AutoMapper;
using ShipVoyageManager.Data.Contracts;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.Port;
using ShipVoyageManager.Data.Object;
using ShipVoyageManager.Service.Contracts;
using System.ComponentModel.DataAnnotations;

namespace ShipVoyageManager.Service.Buisness;
public class PortService : IPortService
{
    private readonly IPortRepository _portRepository;
    private readonly IMapper _mapper;

    public PortService(IPortRepository portRepository, IMapper mapper)
    {
        _portRepository = portRepository;
        _mapper = mapper;
    }

    public async Task<FilteredPaginatedPortsDto> GetFilteredPortsPaginatedAsync(int page, int pageSize, string search)
    {
        return await _portRepository.GetFilteredPortsPaginatedAsync(page, pageSize, search);
    }

    public async Task<List<PortDto>> GetAllPortsAsync()
    {
        return _mapper.Map<List<PortDto>>(await _portRepository.GetAllPortsAsync());
    }

    public async Task AddPortAsync(PortDto portDto)
    {
        var existingPort = await _portRepository.GetPortByNameAsync(portDto.Name);

        if (existingPort != null)
        {
            throw new ValidationException("Port with this name already exists. Please choose a different name.");
        }

        var portEntity = _mapper.Map<PortEntity>(portDto);

        await _portRepository.AddPortAsync(portEntity);
    }

    public Task UpdatePortAsync(PortDto portDto)
    {
        var newPortEntity = _mapper.Map<PortEntity>(portDto);
        return _portRepository.UpdatePortAsync(newPortEntity);
    }

    public async Task DeletePortByIdAsync(Guid id)
    {
        await _portRepository.DeletePortByIdAsync(id);
    }
}
