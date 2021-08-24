using System.Threading.Tasks;

namespace Vehicles.API.Data
{
    public class SeedDb
    {
        private readonly DataContext _dataContext;

        public SeedDb(DataContext context)
        {
            _dataContext = context;
        }
        public async Task SeedAsync()
        {
            await _dataContext.Database.EnsureCreatedAsync();
            
        }
    }
}
