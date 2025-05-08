using ShipVoyageManager.Data.Contracts.Helpers.DTO.Port;

namespace ShipVoyageManager.Data.Contracts.Helpers.DTO.Voyage;
public class PaginatedVoyagesDto
{
    public List<VoyageDto> Voyages { get; set; }
    public int TotalCount { get; set; }
}
