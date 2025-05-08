namespace ShipVoyageManager.Data.Contracts.Helpers.DTO.Port;
public class VoyageDto
{
    public Guid? Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public Guid ShipId { get; set; }
    public string? ShipName { get; set; }
    public Guid DeparturePortId { get; set; }
    public string? DeparturePortName { get; set; }
    public Guid ArrivalPortId { get; set; }
    public string? ArrivalPortName { get; set; }
}
