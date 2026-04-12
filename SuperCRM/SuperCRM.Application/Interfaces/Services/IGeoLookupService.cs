using SuperCRM.Application.DTOs.Lookups;

namespace SuperCRM.Application.Interfaces.Services
{
    public interface IGeoLookupService
    {
        Task<List<CountryLookupDto>> GetCountriesAsync(CancellationToken cancellationToken = default);
        Task<List<RegionLookupDto>> GetRegionsByCountryIdAsync(int countryId, CancellationToken cancellationToken = default);
        Task<List<CityLookupDto>> GetCitiesByRegionIdAsync(int regionId, CancellationToken cancellationToken = default);
    }
}