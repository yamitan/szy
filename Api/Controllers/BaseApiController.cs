using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using szy.Api.Models.Responses;
using szy.Services;

namespace szy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController<T>(
        DatabaseService dbService,
        ILogger<T> logger,
        ConfigService configService) : ControllerBase where T : BaseApiController<T>
    {
        protected readonly DatabaseService DbService = dbService;
        protected readonly ILogger<T> Logger = logger;
        protected readonly ConfigService ConfigService = configService;

        protected IActionResult Success(string message = "操作成功")
        {
            return Ok(ApiResult.Ok(message));
        }

        protected IActionResult Success<TData>(TData data, string message = "操作成功")
        {
            return Ok(ApiResult<TData>.Ok(data, message));
        }

        protected IActionResult Fail(string message = "操作失败", int code = 400)
        {
            return BadRequest(ApiResult.Fail(message, code));
        }
    }
}