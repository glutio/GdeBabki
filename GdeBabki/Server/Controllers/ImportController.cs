using GdeBabki.Server.Services;
using GdeBabki.Shared.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GdeBabki.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] IFormFile file, [FromForm] string filter, [FromForm] Guid accountId, [FromServices] ImportService importService)
        {
            using var stream = file.OpenReadStream();
            await importService.ImportAsync(accountId, stream, filter);

            return Ok();
        }
    }
}
