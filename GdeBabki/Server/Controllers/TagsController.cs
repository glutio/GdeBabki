﻿using GdeBabki.Server.Services;
using GdeBabki.Shared.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GdeBabki.Server.Controllers
{
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
        public async Task<IActionResult> InsertTagAsync(TransactionTag insertTag)
        {
            await tagsService.InsertTagAsync(insertTag);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTagAsync([FromQuery]string tagId, [FromQuery]Guid transactionId)
        {
            await tagsService.DeleteTagAsync(tagId, transactionId);
            return Ok();
        }

    }
}
