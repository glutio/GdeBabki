using GdeBabki.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GdeBabki.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DatabaseService databaseService;

        public UserController(DatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        [HttpGet]
        public IActionResult TestDatabaseConnection()
        {
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateDatabase()
        {
            await databaseService.CreateDatabase();
            return Ok();
        }
    }
}
