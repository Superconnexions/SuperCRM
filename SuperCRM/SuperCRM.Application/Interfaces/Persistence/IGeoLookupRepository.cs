using SuperCRM.Domain.Entities;

namespace SuperCRM.Application.Interfaces.Persistence
{
    public interface IGeoLookupRepository
    {
        Task<List<Country>> GetCountriesAsync(CancellationToken cancellationToken);
        Task<List<Region>> GetRegionsByCountryIdAsync(int countryId, CancellationToken cancellationToken);
        Task<List<City>> GetCitiesByRegionIdAsync(int regionId, CancellationToken cancellationToken);
    }
}