using GdeBabki.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Net;
using System.Threading.Tasks;

namespace GdeBabki.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DatabaseService databaseService;

        public UserController(DatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult TestDatabaseConnection()
        {            
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateDatabase()
        {
            try
            {
                await databaseService.CreateDatabaseAsync();
            } 
            catch (DuplicateNameException)
            {
                return StatusCode((int)HttpStatusCode.Conflict);
            }

            return Ok();
        }
    }
}
