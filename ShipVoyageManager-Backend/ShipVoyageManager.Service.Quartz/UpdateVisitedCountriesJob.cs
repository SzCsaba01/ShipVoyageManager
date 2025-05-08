using Quartz;
using ShipVoyageManager.Service.Contracts;

namespace ShipVoyageManager.Service.Quartz;
public class UpdateVisitedCountriesJob : IJob
{
    private readonly IVisitedCountryService _visitedCountryService;

    public UpdateVisitedCountriesJob(IVisitedCountryService visitedCountryService)
    {
        _visitedCountryService = visitedCountryService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _visitedCountryService.UpdateVisitedCountriesAsync();
    }
}
