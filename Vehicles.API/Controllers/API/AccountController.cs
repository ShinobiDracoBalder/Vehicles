using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Vehicles.API.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration){
            _configuration = configuration;
        }
    }
}
