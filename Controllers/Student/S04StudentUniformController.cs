using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using atmglobalapi.Model.Student;

namespace atmglobalapi.Controllers.Student
{
    [ApiExplorerSettings(GroupName = "Student")]
    [Tags("Student")]
    [Route("api/student/[controller]")]
    [ApiController]
    [Authorize]
    public class S04StudentUniformController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public S04StudentUniformController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("operation")]
        public IActionResult StudentUniformOperation(
            [FromBody] S04StudentUniform model)
        {
            try
            {
                /* ================= JWT CLAIMS ================= */
                int userId = Convert.ToInt32(
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                string roleId = User.FindFirst(ClaimTypes.Role)?.Value;

                /* ================= ROLE CHECK ================= */
                // Admin only for Delete & Archive
                if ((model.Type == 3 || model.Type == 4) && roleId != "1")
                {
                    return Unauthorized(new
                    {
                        isSuccess = false,
                        message = "You are not authorized to perform this action"
                    });
                }

                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("U77_Common")))
                using (SqlCommand cmd = new SqlCommand(
                    "[dbo].[U77_Pro_S04_StudentUniform_Operation]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    /* ================= PARAMETERS ================= */
                    cmd.Parameters.AddWithValue("@Type", model.Type);
                    cmd.Parameters.AddWithValue("@Id", (object?)model.Id ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@StudentId", (object?)model.StudentId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BatchEnrollmentId", (object?)model.BatchEnrollmentId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@GenderId", (object?)model.GenderId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UniformType", (object?)model.UniformType ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Size", (object?)model.Size ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Length", (object?)model.Length ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Chest", (object?)model.Chest ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Waist", (object?)model.Waist ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Hip", (object?)model.Hip ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsReceived", (object?)model.IsReceived ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReceivedDate", (object?)model.ReceivedDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@HandedOverBy", (object?)model.HandedOverBy ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@UserId", userId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                /* ================= RESPONSE ================= */
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
