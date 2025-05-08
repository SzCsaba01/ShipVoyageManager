using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShipVoyageManager.Data.Object;

[Table("Voyages")]
public class VoyageEntity
{
    [Key]
    public Guid Id { get; set; }

    public DateTime VoyageDate { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    [Required]
    public Guid ShipId { get; set; }

    [ForeignKey("ShipId")]
    public ShipEntity Ship { get; set; }

    [Required]
    public Guid DeparturePortId { get; set; }
    [ForeignKey("DeparturePortId")]
    public PortEntity DeparturePort { get; set; }

    [Required]
    public Guid ArrivalPortId { get; set; }
    [ForeignKey("ArrivalPortId")]
    public PortEntity ArrivalPort { get; set; }
}
