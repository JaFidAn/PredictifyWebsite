using Application.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Dynamic;

namespace API.Controllers;

[EnableRateLimiting("fixed")]
[Route("api/[controller]")]
[ApiController]
public class BaseApiController : ControllerBase
{
    protected ActionResult HandleResult<T>(Result<T> result)
    {
        if (!result.IsSuccess)
        {
            if (result.Code == 404)
                return NotFound(new { message = result.Error, code = result.Code });

            return BadRequest(new { message = result.Error, code = result.Code });
        }

        var actionName = ControllerContext.ActionDescriptor.ActionName?.ToLower();
        var type = typeof(T);

        if (actionName == "login")
        {
            return Ok(new
            {
                token = result.Value,
                message = result.Message
            });
        }

        if (type == typeof(string) && (actionName == "register" || actionName == "verify2fa"))
        {
            return Ok(new
            {
                token = result.Value!,
                message = result.Message
            });
        }

        if (type == typeof(string))
        {
            if (string.IsNullOrWhiteSpace(result.Message))
                return Ok(new { message = result.Value });

            return Ok(new { message = result.Message });
        }

        if (type == typeof(Guid) || type == typeof(int))
        {
            dynamic obj = new ExpandoObject();
            ((IDictionary<string, object>)obj)["id"] = result.Value!;
            obj.message = result.Message;

            return Ok(obj);
        }

        if (result.Message == null)
        {
            return Ok(result.Value);
        }

        return Ok(new
        {
            data = result.Value,
            message = result.Message
        });
    }
}
