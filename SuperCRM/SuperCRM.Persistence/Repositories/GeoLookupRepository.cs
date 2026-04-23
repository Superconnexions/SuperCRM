using Microsoft.EntityFrameworkCore;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Domain.Entities;
using SuperCRM.Persistence.DbContexts;

namespace SuperCRM.Persistence.Repositories
{
    public class GeoLookupRepository : IGeoLookupRepository
    {
        private readonly SuperCrmDbContext _dbContext;

        public GeoLookupRepository(SuperCrmDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Country>> GetCountriesAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Countries
                .AsNoTracking()
                .OrderBy(x => x.CountryName)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Region>> GetRegionsByCountryIdAsync(int countryId, CancellationToken cancellationToken)
        {
            return await _dbContext.Regions
                .AsNoTracking()
                .Where(x => x.CountryId == countryId && x.IsActive)
                .OrderBy(x => x.RegionName)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<City>> GetCitiesByRegionIdAsync(int regionId, CancellationToken cancellationToken)
        {
            return await _dbContext.Cities
                .AsNoTracking()
                .Where(x => x.RegionId == regionId && x.IsActive)
                .OrderBy(x => x.CityName)
                .ToListAsync(cancellationToken);
        }
    }
}