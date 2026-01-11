using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace atmglobalapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class HealthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public HealthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "API is running",
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                version = "1.0.0",
                dotnetVersion = Environment.Version.ToString()
            });
        }

        [HttpGet("config")]
        [AllowAnonymous]
        public IActionResult CheckConfig()
        {
            return Ok(new
            {
                hasU77Master = !string.IsNullOrEmpty(_configuration.GetConnectionString("U77_Master")),
                hasU77Common = !string.IsNullOrEmpty(_configuration.GetConnectionString("U77_Common")),
                hasJwtSecret = !string.IsNullOrEmpty(_configuration["JWT:Secret"]),
                jwtIssuer = _configuration["JWT:ValidIssuer"],
                jwtAudience = _configuration["JWT:ValidAudience"],
                serverUrl = _configuration["serverurl"]
            });
        }

        [HttpGet("db-test")]
        [AllowAnonymous]
        public IActionResult TestDatabase()
        {
            var results = new Dictionary<string, object>();

            // Test U77_Master
            try
            {
                using var con = new SqlConnection(_configuration.GetConnectionString("U77_Master"));
                con.Open();
                using var cmd = new SqlCommand("SELECT @@VERSION", con);
                var version = cmd.ExecuteScalar()?.ToString();
                var versionDisplay = version != null 
                    ? version.Substring(0, Math.Min(100, version.Length)) 
                    : "Unknown";
                results["U77_Master"] = new { status = "Connected", version = versionDisplay };
            }
            catch (Exception ex)
            {
                results["U77_Master"] = new { status = "Failed", error = ex.Message };
            }

            // Test U77_Common
            try
            {
                using var con = new SqlConnection(_configuration.GetConnectionString("U77_Common"));
                con.Open();
                using var cmd = new SqlCommand("SELECT @@VERSION", con);
                var version = cmd.ExecuteScalar()?.ToString();
                var versionDisplay = version != null 
                    ? version.Substring(0, Math.Min(100, version.Length)) 
                    : "Unknown";
                results["U77_Common"] = new { status = "Connected", version = versionDisplay };
            }
            catch (Exception ex)
            {
                results["U77_Common"] = new { status = "Failed", error = ex.Message };
            }

            return Ok(new
            {
                isSuccess = true,
                timestamp = DateTime.UtcNow,
                databases = results
            });
        }
    }
}