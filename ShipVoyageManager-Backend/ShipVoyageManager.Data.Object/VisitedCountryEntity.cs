using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShipVoyageManager.Data.Object;

[Table("VisitedCountries")]
public class VisitedCountryEntity
{
    [Key]
    public Guid Id { get; set; }

    public required string CountryName { get; set; }

    [Required(ErrorMessage = "Visited date is required.")]
    public DateTime VisitedDate { get; set; }
}
