namespace ShipVoyageManager.Data.Contracts.Helpers.DTO.Port;
public class FilteredPaginatedPortsDto
{
    public List<PortDto> Ports { get; set; }
    public int TotalCount { get; set; }
}
