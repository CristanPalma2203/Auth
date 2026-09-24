using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/version")]
    [ApiController]
    [AllowAnonymous]
    public class VersionController : ControllerBase
    {
        [HttpGet]
        public object Get() => BuildInfo.Read("auth");
    }

    internal static class BuildInfo
    {
        public static object Read(string service)
        {
            var commit = Environment.GetEnvironmentVariable("GIT_COMMIT");
            if (string.IsNullOrWhiteSpace(commit) || commit == "local") commit = null;
            var shortCommit = commit != null && commit.Length >= 7 ? commit.Substring(0, 7) : commit;
            return new { service, commit, shortCommit };
        }
    }
}
