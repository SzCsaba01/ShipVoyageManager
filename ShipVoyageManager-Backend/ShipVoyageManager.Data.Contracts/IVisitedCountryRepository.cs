using ShipVoyageManager.Data.Object;

namespace ShipVoyageManager.Data.Contracts;
public interface IVisitedCountryRepository
{
    public Task<List<VisitedCountryEntity>> GetAllVisitedCountriesAsync();
    public Task<List<VisitedCountryEntity>> GetVisitedCountriesLastYearAsync();
    public Task AddVisitedCountriesAsync(List<VisitedCountryEntity> visitedCountries);
    public Task DeleteVisitedCountriesAsync(List<VisitedCountryEntity> visitedCountries);
}
