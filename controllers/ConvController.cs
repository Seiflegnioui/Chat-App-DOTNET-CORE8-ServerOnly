using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using p4.Models.DTO;
using p4.Models.Entities;
using p4.Services;

namespace p4.controllers
{
    [ApiController]
    [Route("conversation")]
    public class ConvController(IConversationService conversationService) : ControllerBase
    {
        [HttpPost("start")]
        [Authorize]
        public async Task<IActionResult> start(ConvDTO conv)
        {
            try
            {
                var ID = await conversationService.StartAsyn(conv);
                return Ok(ID);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}