using AutoMapper;
using ShipVoyageManager.Data.Contracts;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.Ship;
using ShipVoyageManager.Data.Object;
using ShipVoyageManager.Service.Contracts;
using System.ComponentModel.DataAnnotations;

namespace ShipVoyageManager.Service.Buisness;

public class ShipService : IShipService
{
    private readonly IShipRepository _shipRepository;
    private readonly IMapper _mapper;

    public ShipService(IShipRepository shipRepository, IMapper mapper)
    {
        _shipRepository = shipRepository;
        _mapper = mapper;
    }

    public async Task<FilteredPaginatedShipsDto> GetFilteredShipsPaginatedAsync(int page, int pageSize, string search)
    {
        return await _shipRepository.GetFilteredShipsPaginatedAsync(page, pageSize, search);
    }

    public async Task<List<ShipDto>> GetShipsOutOfDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return _mapper.Map<List<ShipDto>>(await _shipRepository.GetShipsOutOfDateRangeAsync(startDate, endDate));
    }

    public async Task AddShipAsync(ShipDto shipDto)
    {
        var existingShip = await _shipRepository.GetShipByNameAsync(shipDto.Name);

        if (existingShip != null)
        {
            throw new ValidationException("Ship with this name already exists. Please choose a different name.");
        }

        var shipEntity = _mapper.Map<ShipEntity>(shipDto);

        await _shipRepository.AddShipAsync(shipEntity);
    }

    public async Task UpdateShipAsync(ShipDto shipDto)
    {
        var newShipEntity = _mapper.Map<ShipEntity>(shipDto);

        await _shipRepository.UpdateShipAsync(newShipEntity);
    }

    public async Task DeleteShipByIdAsync(Guid id)
    {
        await _shipRepository.DeleteShipByIdAsync(id);
    }
}
