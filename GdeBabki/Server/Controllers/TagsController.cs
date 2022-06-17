using GdeBabki.Server.Services;
using GdeBabki.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GdeBabki.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly TagsService tagsService;

        public TagsController(TagsService tagsService)
        {
            this.tagsService = tagsService;
        }

        [HttpPost]
        public async Task<IActionResult> InsertTagAsync([FromBody]TransactionTag transactionTag)
        {
            await tagsService.InsertTagAsync(transactionTag);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTagAsync([FromQuery]string tagId, [FromQuery]Guid transactionId)
        {
            await tagsService.DeleteTagAsync(tagId, transactionId);
            return Ok();
        }

        [HttpGet, Route("Suggested/{transactionId=transactionId}")]
        public async Task<ActionResult<string[]>> GetSuggestedTagsAsync([FromQuery]Guid transactionId)
        {
            var result = await tagsService.GetSuggestedTagsAsync(transactionId);
            return new JsonResult(result);
        }

        [HttpPost("Shared")]
        public async Task<IActionResult> InsertSharedTagAsync([FromBody]SharedTag sharedTag)
        {
            await tagsService.InsertSharedTagAsync(sharedTag);
            return Ok();
        }

        [HttpDelete("Shared")]
        public async Task<IActionResult> DeleteSharedTagAsync([FromBody]SharedTag sharedTag)
        {
            await tagsService.DeleteSharedTagAsync(sharedTag);
            return Ok();
        }

    }
}
