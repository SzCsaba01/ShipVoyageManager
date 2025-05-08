using ShipVoyageManager.Data.Contracts.Helpers.DTO.Port;

namespace ShipVoyageManager.Data.Contracts.Helpers.DTO.Ship;
public class ShipDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; }
    public double MaxSpeed { get; set; }
    public ICollection<VoyageDto>? Voyages { get; set; }
}
