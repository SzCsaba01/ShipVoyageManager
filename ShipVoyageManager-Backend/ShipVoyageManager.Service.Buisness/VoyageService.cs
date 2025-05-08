using AutoMapper;
using ShipVoyageManager.Data.Contracts;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.Port;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.Voyage;
using ShipVoyageManager.Data.Object;
using ShipVoyageManager.Service.Buisness.Exceptions;
using ShipVoyageManager.Service.Contracts;

namespace ShipVoyageManager.Service.Buisness;
public class VoyageService : IVoyageService
{
    private readonly IVoyageRepository _voyageRepository;
    private readonly IMapper _mapper;

    public VoyageService(IVoyageRepository voyageRepository, IMapper mapper)
    {
        _voyageRepository = voyageRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedVoyagesDto> GetVoyagesPaginatedAsync(int page, int pageSize)
    {
        return await _voyageRepository.GetVoyagesPaginatedAsync(page, pageSize);
    }

    public async Task AddVoyageAsync(VoyageDto voyageDto)
    {
        var voyageEntity = _mapper.Map<VoyageEntity>(voyageDto);

        await _voyageRepository.AddVoyageAsync(voyageEntity);
    }

    public async Task UpdateVoyageAsync(VoyageDto voyageDto)
    {
        var newVoyageEntity = _mapper.Map<VoyageEntity>(voyageDto);
        await _voyageRepository.UpdateVoyageAsync(newVoyageEntity);
    }

    public async Task DeleteVoyageAsync(Guid id)
    {
        await _voyageRepository.DeleteVoyageByIdAsync(id);
    }

}
