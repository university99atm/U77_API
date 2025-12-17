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
    public class S06StudentParentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public S06StudentParentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("operation")]
        public IActionResult StudentParentOperation(
            [FromBody] S06StudentParent model)
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
                    "[dbo].[U77_Pro_S06_StudentParent_Operation]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    /* ================= PARAMETERS ================= */
                    cmd.Parameters.AddWithValue("@Type", model.Type);
                    cmd.Parameters.AddWithValue("@Id", (object?)model.Id ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@StudentId", (object?)model.StudentId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@RelationId", (object?)model.RelationId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FullName", (object?)model.FullName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MobileNo", (object?)model.MobileNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MobileNo2", (object?)model.MobileNo2 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmailId", (object?)model.EmailId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LandlineNo", (object?)model.LandlineNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AadhaarNo", (object?)model.AadhaarNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Qualification", (object?)model.Qualification ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Profession", (object?)model.Profession ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Designation", (object?)model.Designation ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyName", (object?)model.CompanyName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@OfficeAddress", (object?)model.OfficeAddress ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Industry", (object?)model.Industry ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FunctionalArea", (object?)model.FunctionalArea ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@RoleName", (object?)model.RoleName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AnnualIncome", (object?)model.AnnualIncome ?? DBNull.Value);

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
