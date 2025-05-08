using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShipVoyageManager.Data.Object;

[Table("Ports")]
public class PortEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Port name is required.")]
    [MaxLength(100, ErrorMessage = "Port name can be maximum 100 characters long.")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Port location is required.")]
    [MaxLength(100, ErrorMessage = "Port location can be maximum 100 characters long.")]
    public required string CountryName { get; set; }

    public ICollection<VoyageEntity>? DepartingVoyages { get; set; }
    public ICollection<VoyageEntity>? ArrivingVoyages { get; set; }
}
