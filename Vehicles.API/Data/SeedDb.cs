using System.Linq;
using System.Threading.Tasks;
using Vehicles.API.Data.Entities;
using Vehicles.API.Helpers;
using Vehicles.Common.Enums;

namespace Vehicles.API.Data
{
    public class SeedDb
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _dataContext = context;
            _userHelper = userHelper;
        }
        public async Task SeedAsync()
        {
            await _dataContext.Database.EnsureCreatedAsync();
            await CheckVehiclesTypeAsync();
            await CheckBrandsAsync();
            await CheckDocumentTypesAsync();
            await CheckProceduresAsync();
            await CheckRolesAsycn();
            await CheckUserAsync("1010", "Luis", "Salazar", "luis@yopmail.com", "311 322 4620", "Calle Luna Calle Sol", UserType.Admin);
            await CheckUserAsync("2020", "Juan", "Zuluaga", "zulu@yopmail.com", "311 322 4620", "Calle Luna Calle Sol", UserType.User);
            await CheckUserAsync("3030", "Ledys", "Bedoya", "ledys@yopmail.com", "311 322 4620", "Calle Luna Calle Sol", UserType.User);
            await CheckUserAsync("4040", "Sandra", "Lopera", "sandra@yopmail.com", "311 322 4620", "Calle Luna Calle Sol", UserType.User);
            await CheckUserAsync("911", "Draco", "Master", "draco.master@yopmail.com", "311 322 4620", "Calle Luna Calle Sol", UserType.Admin);
        }
        private async Task CheckRolesAsycn()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }
        private async Task CheckUserAsync(string document, string firstName, string lastName, string email, string phoneNumber, string address, UserType userType)
        {
            User user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                user = new User
                {
                    Address = address,
                    Document = document,
                    DocumentType = _dataContext.DocumentTypes.FirstOrDefault(x => x.Description == "Cédula"),
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = phoneNumber,
                    UserName = email,
                    UserType = userType
                };

                await _userHelper.AddUserAsync(user, "D123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }
        }

        private async Task CheckProceduresAsync()
        {
            if (!_dataContext.Procedures.Any()){
                _dataContext.Procedures.Add(new Procedure { Price = 10000, Description = "Alineación" });
                _dataContext.Procedures.Add(new Procedure { Price = 10000, Description = "Lubricación de suspención delantera" });
                _dataContext.Procedures.Add(new Procedure { Price = 10000, Description = "Lubricación de suspención trasera" });
                _dataContext.Procedures.Add(new Procedure { Price = 10000, Description = "Frenos delanteros" });
                _dataContext.Procedures.Add(new Procedure { Price = 10000, Description = "Frenos traseros" });
                _dataContext.Procedures.Add(new Procedure { Price = 10000, Description = "Líquido frenos delanteros" });
                _dataContext.Procedures.Add(new Procedure { Price = 10000, Description = "Líquido frenos traseros" });
                _dataContext.Procedures.Add(new Procedure { Price = 10000, Description = "Calibración de válvulas" });
                _dataContext.Procedures.Add(new Procedure { Price = 10000, Description = "Alineación carburador" });
                _dataContext.Procedures.Add(new Procedure { Price = 10000, Description = "Aceite motor" });
                _dataContext.Procedures.Add(new Procedure { Price = 10000, Description = "Aceite caja" });
                _dataContext.Procedures.Add(new Procedure { Price = 10000, Description = "Filtro de aire" });
                _dataContext.Procedures.Add(new Procedure { Price = 10000, Description = "Sistema eléctrico" });
                _dataContext.Procedures.Add(new Procedure { Price = 10000, Description = "Guayas" });
                _dataContext.Procedures.Add(new Procedure { Price = 10000, Description = "Cambio llanta delantera" });
                _dataContext.Procedures.Add(new Procedure { Price = 10000, Description = "Cambio llanta trasera" });
                _dataContext.Procedures.Add(new Procedure { Price = 10000, Description = "Reparación de motor" });
                _dataContext.Procedures.Add(new Procedure { Price = 10000, Description = "Kit arrastre" });
                _dataContext.Procedures.Add(new Procedure { Price = 10000, Description = "Banda transmisión" });
                _dataContext.Procedures.Add(new Procedure { Price = 10000, Description = "Cambio batería" });
                _dataContext.Procedures.Add(new Procedure { Price = 10000, Description = "Lavado sistema de inyección" });
                _dataContext.Procedures.Add(new Procedure { Price = 10000, Description = "Lavada de tanque" });
                _dataContext.Procedures.Add(new Procedure { Price = 10000, Description = "Cambio de bujia" });
                _dataContext.Procedures.Add(new Procedure { Price = 10000, Description = "Cambio rodamiento delantero" });
                _dataContext.Procedures.Add(new Procedure { Price = 10000, Description = "Cambio rodamiento trasero" });
                _dataContext.Procedures.Add(new Procedure { Price = 10000, Description = "Accesorios" });
                await _dataContext.SaveChangesAsync();
            }
        }

        private async Task CheckDocumentTypesAsync()
        {
            if (!_dataContext.DocumentTypes.Any()){
                _dataContext.DocumentTypes.Add(new DocumentType { Description = "Cédula" });
                _dataContext.DocumentTypes.Add(new DocumentType { Description = "Tarjeta de Identidad" });
                _dataContext.DocumentTypes.Add(new DocumentType { Description = "NIT" });
                _dataContext.DocumentTypes.Add(new DocumentType { Description = "Pasaporte" });
                await _dataContext.SaveChangesAsync();
            }
        }

        private async Task CheckBrandsAsync()
        {
            if (!_dataContext.Brands.Any()){
                _dataContext.Brands.Add(new Brand { Description = "Ducati" });
                _dataContext.Brands.Add(new Brand { Description = "Harley Davidson" });
                _dataContext.Brands.Add(new Brand { Description = "KTM" });
                _dataContext.Brands.Add(new Brand { Description = "BMW" });
                _dataContext.Brands.Add(new Brand { Description = "Triumph" });
                _dataContext.Brands.Add(new Brand { Description = "Victoria" });
                _dataContext.Brands.Add(new Brand { Description = "Honda" });
                _dataContext.Brands.Add(new Brand { Description = "Suzuki" });
                _dataContext.Brands.Add(new Brand { Description = "Kawasaky" });
                _dataContext.Brands.Add(new Brand { Description = "TVS" });
                _dataContext.Brands.Add(new Brand { Description = "Bajaj" });
                _dataContext.Brands.Add(new Brand { Description = "AKT" });
                _dataContext.Brands.Add(new Brand { Description = "Yamaha" });
                _dataContext.Brands.Add(new Brand { Description = "Chevrolet" });
                _dataContext.Brands.Add(new Brand { Description = "Mazda" });
                _dataContext.Brands.Add(new Brand { Description = "Renault" });
                await _dataContext.SaveChangesAsync();
            }
        }

        private async Task CheckVehiclesTypeAsync()
        {
            if (!_dataContext.VehicleTypes.Any()){
                _dataContext.VehicleTypes.Add(new VehicleType { Description = "Carro" });
                _dataContext.VehicleTypes.Add(new VehicleType { Description = "Moto" });
                await _dataContext.SaveChangesAsync();
            }
        }
    }
}
