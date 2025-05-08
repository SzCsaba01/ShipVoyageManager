using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShipVoyageManager.Data.Object;

[Table("Ships")]
public class ShipEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Ship name is required.")]
    [MaxLength(100, ErrorMessage ="Ship name can be maximum 100 characters long.")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Ship max speed is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "Ship max speed must be a positive number.")]
    public required double MaxSpeed { get; set; }

    public ICollection<VoyageEntity>? Voyages { get; set; }
}
