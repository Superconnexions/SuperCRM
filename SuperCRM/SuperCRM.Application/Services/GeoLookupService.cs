using SuperCRM.Application.DTOs.Lookups;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Application.Interfaces.Services;

namespace SuperCRM.Application.Services
{
    public class GeoLookupService : IGeoLookupService
    {
        private readonly IGeoLookupRepository _repository;

        public GeoLookupService(IGeoLookupRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CountryLookupDto>> GetCountriesAsync(CancellationToken cancellationToken)
        {
            var data = await _repository.GetCountriesAsync(cancellationToken);

            return data.Select(x => new CountryLookupDto
            {
                Id = x.CountryId,
                Name = x.CountryName
            }).ToList();
        }

        public async Task<List<RegionLookupDto>> GetRegionsByCountryIdAsync(int countryId, CancellationToken cancellationToken)
        {
            var data = await _repository.GetRegionsByCountryIdAsync(countryId, cancellationToken);

            return data.Select(x => new RegionLookupDto
            {
                Id = x.RegionId,
                Name = x.RegionName
            }).ToList();
        }

        public async Task<List<CityLookupDto>> GetCitiesByRegionIdAsync(int regionId, CancellationToken cancellationToken)
        {
            var data = await _repository.GetCitiesByRegionIdAsync(regionId, cancellationToken);

            return data.Select(x => new CityLookupDto
            {
                Id = x.CityId,
                Name = x.CityName
            }).ToList();
        }
    }
}