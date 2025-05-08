using AutoMapper;
using ShipVoyageManager.Data.Contracts;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.VisitedCountry;
using ShipVoyageManager.Data.Object;
using ShipVoyageManager.Service.Contracts;

namespace ShipVoyageManager.Service.Buisness;
public class VisitedCountryService : IVisitedCountryService
{
    private readonly IVisitedCountryRepository _visitedCountryRepository;
    private readonly IMapper _mapper;

    public VisitedCountryService(IVisitedCountryRepository visitedCountryRepository, IMapper mapper)
    {
        _visitedCountryRepository = visitedCountryRepository;
        _mapper = mapper;
    }

    public async Task<List<VisitedCountryDto>> GetAllVisitedCountriesAsync()
    {
        var visitedCountries = await _visitedCountryRepository.GetAllVisitedCountriesAsync();

        return _mapper.Map<List<VisitedCountryDto>>(visitedCountries);
    }

    public async Task UpdateVisitedCountriesAsync()
    {
        var currentVisitedCountries = await _visitedCountryRepository.GetAllVisitedCountriesAsync();
        var visitedCountriesLastYear = await _visitedCountryRepository.GetVisitedCountriesLastYearAsync();

        var visitedCountriesToAdd = visitedCountriesLastYear
            .Where(vc => !currentVisitedCountries.Any(c => c.CountryName == vc.CountryName && c.VisitedDate == vc.VisitedDate))
            .ToList();

        var visitedCountriesToDelete = currentVisitedCountries
            .Where(vc => !visitedCountriesLastYear.Any(c => c.CountryName == vc.CountryName && c.VisitedDate == vc.VisitedDate))
            .ToList();

        if (visitedCountriesToDelete.Any())
        {
            await _visitedCountryRepository.DeleteVisitedCountriesAsync(visitedCountriesToDelete);
        }

        if (visitedCountriesToAdd.Any())
        {
            await _visitedCountryRepository.AddVisitedCountriesAsync(visitedCountriesToAdd);
        }
    }
}
