using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using szy.Api.Models.Responses;
using szy.Services;
using System.Data.SQLite;

namespace szy.Api.Controllers
{
    public class SystemController : BaseApiController<SystemController>
    {
        public SystemController(
            DatabaseService dbService,
            ILogger<SystemController> logger,
            ConfigService configService)
            : base(dbService, logger, configService)
        {
        }

        [HttpGet("info")]
        public IActionResult GetSystemInfo()
        {
            var info = new
            {
                ServerTime = DateTime.Now,
                ServerName = Environment.MachineName,
                OsVersion = Environment.OSVersion.ToString(),
                FrameworkVersion = Environment.Version.ToString()
            };

            return Success(info);
        }

        [HttpGet("health")]
        public IActionResult CheckHealth()
        {
            return Success("系统运行正常");
        }

        [HttpGet("db/info")]
        public IActionResult GetDatabaseInfo()
        {
            try
            {
                var dbInfo = DbService.GetDatabaseInfo();
                return Success(dbInfo);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "获取数据库信息失败");
                return Fail("获取数据库信息失败: " + ex.Message);
            }
        }
    }
}