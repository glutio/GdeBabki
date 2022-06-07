using GdeBabki.Server.Services;
using GdeBabki.Shared;
using GdeBabki.Shared.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        public async Task<IActionResult> Post([FromForm] IEnumerable<IFormFile> files, [FromForm] string filter, [FromForm] Guid accountId, [FromServices] ImportService importService)
        {
            using var stream = files.First().OpenReadStream();

            GBColumnName?[] columns = filter.Split(",")
                .Select(e => string.IsNullOrEmpty(e) ? null : (GBColumnName?)int.Parse(e))
                .ToArray();

            await importService.ImportAsync(accountId, stream, columns);

            return Ok();
        }
    }
}
