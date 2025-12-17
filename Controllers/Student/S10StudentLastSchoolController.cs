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
    public class S10StudentLastSchoolController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public S10StudentLastSchoolController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("operation")]
        public IActionResult StudentLastSchoolOperation(
            [FromBody] S10StudentLastSchool model)
        {
            try
            {
                /* ================= JWT ================= */
                int userId = Convert.ToInt32(
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                string roleId = User.FindFirst(ClaimTypes.Role)?.Value;

                /* ================= ROLE CHECK ================= */
                // Delete & Archive → Admin only
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
                    "[dbo].[U77_Pro_S10_StudentLastSchool_Operation]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    /* ================= PARAMETERS ================= */
                    cmd.Parameters.AddWithValue("@Type", model.Type);
                    cmd.Parameters.AddWithValue("@Id", (object?)model.Id ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@StudentId", (object?)model.StudentId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@InstituteId", (object?)model.InstituteId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SectorId", (object?)model.SectorId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BatchCode", (object?)model.BatchCode ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@PrincipalName", (object?)model.PrincipalName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PrincipalMobile", (object?)model.PrincipalMobile ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PrincipalEmail", (object?)model.PrincipalEmail ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@BestSchoolTeacherName", (object?)model.BestSchoolTeacherName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BestSchoolTeacherMobile", (object?)model.BestSchoolTeacherMobile ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BestSchoolTeacherEmail", (object?)model.BestSchoolTeacherEmail ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@BestCoachingTeacherName", (object?)model.BestCoachingTeacherName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BestCoachingTeacherMobile", (object?)model.BestCoachingTeacherMobile ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BestCoachingTeacherEmail", (object?)model.BestCoachingTeacherEmail ?? DBNull.Value);

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
