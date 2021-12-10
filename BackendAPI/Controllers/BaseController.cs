using System;
using System.Net;
using BackendAPI.Modules;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController: ControllerBase
    {
        protected ActionResult OkResponse(object result)
        {
            return Ok(new
            {
                StatusCode = HttpStatusCode.OK,
                Data = result
            });
        }

        protected ActionResult UnauthorizedResponse(object result)
        {
            return Unauthorized(new
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Data = result
            });
        }
        protected ActionResult NotFoundResponse(object result)
        {
            return NotFound(new
            {
                StatusCode = HttpStatusCode.NotFound,
                Data = result
            });
        }
        protected ActionResult BadRequestResponse(object result)
        {
            return BadRequest(new
            {
                StatusCode = HttpStatusCode.BadRequest,
                Data = result
            });
        }
        protected ActionResult ConflictResponse(object result)
        {
            return Conflict(new
            {
                StatusCode = HttpStatusCode.Conflict,
                Data = result
            });
        }

        protected ObjectResult ForbiddenResponse(object result)
        {
            return StatusCode(((int)HttpStatusCode.Forbidden), new
            {
                StatusCode = HttpStatusCode.Conflict,
                Data = result
            });
        }
    }
}
