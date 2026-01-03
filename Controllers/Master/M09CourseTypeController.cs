using atmglobalapi.Model.Master;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Linq;
using System.Security.Claims;

namespace atmglobalapi.Controllers.Master
{
    [ApiExplorerSettings(GroupName = "Master")]
    [Tags("Master")]
    [Route("api/master/[controller]")]
    [ApiController]
    [Authorize]
    public class M09CourseTypeController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public M09CourseTypeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("operation")]
        public IActionResult CourseTypeOperation([FromBody] M09CourseType model)
        {
            try
            {
                int userId = Convert.ToInt32(
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                string roleId =
                    User.FindFirst(ClaimTypes.Role)?.Value ?? "0";

                if ((model.Type == 3 || model.Type == 8) && roleId != "1")
                {
                    return Unauthorized(new
                    {
                        isSuccess = false,
                        message = "You are not authorized"
                    });
                }

                string ipAddress =
                    HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                    ?? HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString()
                    ?? "UNKNOWN";

                DataTable dt = new DataTable();

                using SqlConnection con =
                    new SqlConnection(_configuration.GetConnectionString("U77_Common"));
                using SqlCommand cmd =
                    new SqlCommand("dbo.U77_Pro_M09_coursetypeoperation", con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Type", model.Type);
                cmd.Parameters.AddWithValue("@Id", (object?)model.Id ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CourseTypeName", (object?)model.CourseTypeName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", (object?)model.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", (object?)model.Status ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@PageNumber", model.PageNumber ?? 1);
                cmd.Parameters.AddWithValue("@PageSize", model.PageSize ?? 10);
                cmd.Parameters.AddWithValue("@Search", (object?)model.Search ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@System", model.System ?? false);
                cmd.Parameters.AddWithValue("@IPAddress", ipAddress);
                cmd.Parameters.AddWithValue("@OperationBy", userId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    var rows = dt.Rows.Cast<DataRow>()
                        .Select(r => dt.Columns.Cast<DataColumn>()
                            .ToDictionary(
                                c => c.ColumnName,
                                c => r[c] == DBNull.Value ? null : r[c]
                            ))
                        .ToList();

                    return Ok(new { isSuccess = true, data = rows });
                }

                return Ok(new { isSuccess = false, message = "No data found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    isSuccess = false,
                    message = "Internal server error",
                    error = ex.Message
                });
            }
        }
    }
}
