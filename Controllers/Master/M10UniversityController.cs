using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using atmglobalapi.Model.Master;

namespace atmglobalapi.Controllers.Master
{
    [ApiExplorerSettings(GroupName = "Master")]
    [Tags("Master")]
    [Route("api/master/[controller]")]
    [ApiController]
    [Authorize]
    public class M10UniversityController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public M10UniversityController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("operation")]
        public IActionResult UniversityOperation([FromBody] M10University model)
        {
            try
            {
                // 🔐 Admin-only for Delete & Archive
                string roleId = User.FindFirst(ClaimTypes.Role)?.Value;

                if ((model.Type == 3 || model.Type == 4) && roleId != "1")
                {
                    return Unauthorized(new
                    {
                        isSuccess = false,
                        message = "You are not authorized to perform this action"
                    });
                }

                DataTable dt = new DataTable();

                using (SqlConnection con =
                       new SqlConnection(_configuration.GetConnectionString("U77_Common")))
                using (SqlCommand cmd =
                       new SqlCommand("[dbo].[U77_Pro_M10_universityoperation]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Type", model.Type);
                    cmd.Parameters.AddWithValue("@Id", (object?)model.Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CourseTypeId", (object?)model.CourseTypeId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UniversityName", (object?)model.UniversityName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UniversityCode", (object?)model.UniversityCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", (object?)model.Status ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Archive", (object?)model.Archive ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@is_active", (object?)model.is_active ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Created_by", (object?)model.Created_by ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Updated_by", (object?)model.Updated_by ?? DBNull.Value);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                // ✅ Standard Response
                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];

                    return Ok(new
                    {
                        isSuccess = Convert.ToBoolean(row["isSuccess"]),
                        message = row["message"]?.ToString(),
                        data = row["data"] == DBNull.Value
                            ? null
                            : System.Text.Json.JsonSerializer.Deserialize<object>(
                                row["data"].ToString())
                    });
                }

                return Ok(new
                {
                    isSuccess = false,
                    message = "No response from database",
                    data = ""
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
