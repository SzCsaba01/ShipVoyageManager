namespace ShipVoyageManager.Data.Contracts.Helpers.DTO.Port;
public class PortDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; }
    public string CountryName { get; set; }
    public ICollection<VoyageDto>? DepartingVoyages { get; set; }
    public ICollection<VoyageDto>? ArrivingVoyages { get; set; }
}
