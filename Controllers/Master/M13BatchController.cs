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
    public class M13BatchController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public M13BatchController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("operation")]
        public IActionResult BatchOperation([FromBody] M13Batch model)
        {
            try
            {
                /* ================= JWT CLAIMS ================= */
                int userId = Convert.ToInt32(
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                string roleId =
                    User.FindFirst(ClaimTypes.Role)?.Value ?? "0";

                /* ================= ROLE CHECK ================= */
                if ((model.Type == 3 || model.Type == 8) && roleId != "1")
                {
                    return Unauthorized(new
                    {
                        isSuccess = false,
                        message = "You are not authorized to perform this action"
                    });
                }

                /* ================= CLIENT IP ================= */
                string ipAddress =
                    HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                    ?? HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString()
                    ?? "UNKNOWN";

                DataTable dt = new DataTable();

                using (SqlConnection con =
                    new SqlConnection(_configuration.GetConnectionString("U77_Master")))
                using (SqlCommand cmd =
                    new SqlCommand("dbo.U77_Pro_M13_BatchOperation", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Type", model.Type);
                    cmd.Parameters.AddWithValue("@Id", (object?)model.Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SectorId", (object?)model.SectorId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UniversityId", (object?)model.UniversityId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CourseTypeId", (object?)model.CourseTypeId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CourseId", (object?)model.CourseId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CollegeId", (object?)model.CollegeId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BranchId", (object?)model.BranchId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BatchName", (object?)model.BatchName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BatchType", (object?)model.BatchType ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@StartDate", (object?)model.StartDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EndDate", (object?)model.EndDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BatchStatus", (object?)model.BatchStatus ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", (object?)model.Status ?? DBNull.Value);

                    // Pagination
                    cmd.Parameters.AddWithValue("@PageNumber", model.PageNumber ?? 1);
                    cmd.Parameters.AddWithValue("@PageSize", model.PageSize ?? 10);
                    cmd.Parameters.AddWithValue("@Search", (object?)model.Search ?? DBNull.Value);

                    // Audit
                    cmd.Parameters.AddWithValue("@System", model.System ?? false);
                    cmd.Parameters.AddWithValue("@IPAddress", ipAddress);
                    cmd.Parameters.AddWithValue("@OperationBy", userId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                /* ================= RESPONSE ================= */
                if (dt.Rows.Count > 0)
                {
                    // Convert DataTable to a JSON-serializable structure (List<Dictionary<string, object>>)
                    var rows = dt.Rows.Cast<DataRow>()
                        .Select(r => dt.Columns.Cast<DataColumn>()
                            .ToDictionary(
                                c => c.ColumnName,
                                c => r[c] == DBNull.Value ? null : r[c]
                            )
                        )
                        .ToList();

                    return Ok(new
                    {
                        isSuccess = true,
                        data = rows
                    });
                }

                return Ok(new
                {
                    isSuccess = false,
                    message = "No data returned"
                });
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