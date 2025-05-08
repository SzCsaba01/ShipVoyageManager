namespace ShipVoyageManager.Data.Contracts.Helpers.DTO.Ship;
public class FilteredPaginatedShipsDto
{
    public List<ShipDto> Ships { get; set; }
    public int TotalCount { get; set; }
}
