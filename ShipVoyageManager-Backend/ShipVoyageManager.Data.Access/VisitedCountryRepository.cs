using Microsoft.EntityFrameworkCore;
using ShipVoyageManager.Data.Access.Data;
using ShipVoyageManager.Data.Contracts;
using ShipVoyageManager.Data.Object;

namespace ShipVoyageManager.Data.Access;
public class VisitedCountryRepository : IVisitedCountryRepository
{
    private readonly ShipVoyageManagerContext _context;

    public VisitedCountryRepository(ShipVoyageManagerContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<List<VisitedCountryEntity>> GetAllVisitedCountriesAsync()
    {
        return await _context.VisitedCountries
            .AsNoTracking()
            .OrderBy(vc => vc.VisitedDate)
            .ToListAsync();
    }

    public async Task<List<VisitedCountryEntity>> GetVisitedCountriesLastYearAsync()
    {
        var oneYearAgo = DateTime.UtcNow.AddYears(-1);

        var arrivalVisits = _context.Voyages
            .Where(v => v.EndTime >= oneYearAgo)
            .Select(v => new VisitedCountryEntity
            {
                CountryName = v.ArrivalPort.CountryName,
                VisitedDate = v.EndTime.Date
            });

        var departureVisits = _context.Voyages
            .Where(v => v.StartTime >= oneYearAgo)
            .Select(v => new VisitedCountryEntity
            {
                CountryName = v.DeparturePort.CountryName,
                VisitedDate = v.StartTime.Date
            });

        var combinedList = await arrivalVisits
            .Union(departureVisits)
            .ToListAsync();

        var latestVisits = combinedList
            .GroupBy(vc => vc.CountryName)
            .Select(g => new VisitedCountryEntity
            {
                CountryName = g.Key,
                VisitedDate = g.Max(x => x.VisitedDate)
            })
            .ToList();

        return latestVisits;
    }

    public async Task AddVisitedCountriesAsync(List<VisitedCountryEntity> visitedCountries)
    {
        await _context.VisitedCountries.AddRangeAsync(visitedCountries);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteVisitedCountriesAsync(List<VisitedCountryEntity> visitedCountries)
    {
        _context.VisitedCountries.RemoveRange(visitedCountries);
        await _context.SaveChangesAsync();
    }
}
