using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using p4.Models.DTO;
using p4.Models.Entities;
using p4.Services;

namespace p4.controllers
{
    [ApiController]
    [Route("message")]
    public class MessageController(IMessageService messageService, ILogger<MessageController> log) : ControllerBase
    {
        [HttpPost("send")]
        [Authorize]
        public async Task<IActionResult> Send(MesssageDTO msg)
        {
            try
            {
                var saved_msg = await messageService.Send(msg);
                return Ok(saved_msg);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex?.Message);
            }
        }

        [HttpGet("unseen/{myId}")]
        [Authorize]
        public async Task<IActionResult> GetUnseen(int myId)
        {
            try
            {
                var obj = await messageService.GetUnseenMessagesAsync(myId);
                return Ok(obj);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex?.Message);
            }
        }

        [HttpGet("all/{conv}")]
        public async Task<IActionResult> All(int conv)
        {
            try
            {
                var msgs = await messageService.All(conv);
                return Ok(msgs);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex?.Message);
            }
        }
        
    }
}