using ShipVoyageManager.Data.Contracts.Helpers.DTO.VisitedCountry;

namespace ShipVoyageManager.Service.Contracts;
public interface IVisitedCountryService
{
    public Task<List<VisitedCountryDto>> GetAllVisitedCountriesAsync();
    public Task UpdateVisitedCountriesAsync();
}
