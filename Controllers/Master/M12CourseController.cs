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
    public class M12CourseController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public M12CourseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("operation")]
        public IActionResult CourseOperation([FromBody] M12Course model)
        {
            try
            {
                int userId = Convert.ToInt32(
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                string roleId =
                    User.FindFirst(ClaimTypes.Role)?.Value ?? "0";

                if ((model.Type == 3 || model.Type == 8) && roleId != "1")
                {
                    return Unauthorized(new { isSuccess = false, message = "Not authorized" });
                }

                string ipAddress =
                    HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                    ?? HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString()
                    ?? "UNKNOWN";

                DataTable dt = new DataTable();

                using SqlConnection con =
                    new SqlConnection(_configuration.GetConnectionString("U77_Common"));
                using SqlCommand cmd =
                    new SqlCommand("dbo.U77_Pro_M12_courseoperation", con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Type", model.Type);
                cmd.Parameters.AddWithValue("@Id", (object?)model.Id ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CourseTypeId", (object?)model.CourseTypeId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CollegeId", (object?)model.CollegeId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MinQualificationId", (object?)model.MinQualificationId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CourseName", (object?)model.CourseName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CourseCode", (object?)model.CourseCode ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DurationYears", (object?)model.DurationYears ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", (object?)model.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", (object?)model.Status ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@PageNumber", model.PageNumber ?? 1);
                cmd.Parameters.AddWithValue("@PageSize", model.PageSize ?? 10);
                cmd.Parameters.AddWithValue("@Search", (object?)model.Search ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@CourseTypeFilter", (object?)model.CourseTypeFilter ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CollegeFilter", (object?)model.CollegeFilter ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@System", model.System ?? false);
                cmd.Parameters.AddWithValue("@IPAddress", ipAddress);
                cmd.Parameters.AddWithValue("@OperationBy", userId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    var rows = dt.Rows.Cast<DataRow>()
                        .Select(r => dt.Columns.Cast<DataColumn>()
                            .ToDictionary(c => c.ColumnName,
                                c => r[c] == DBNull.Value ? null : r[c]))
                        .ToList();

                    return Ok(new { isSuccess = true, data = rows });
                }

                return Ok(new { isSuccess = false, message = "No data found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { isSuccess = false, error = ex.Message });
            }
        }
    }
}
