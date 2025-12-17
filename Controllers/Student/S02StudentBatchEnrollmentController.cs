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
    public class S02StudentBatchEnrollmentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public S02StudentBatchEnrollmentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("operation")]
        public IActionResult StudentBatchEnrollmentOperation(
            [FromBody] S02StudentBatchEnrollment model)
        {
            try
            {
                /* ================= JWT CLAIMS ================= */
                int userId = Convert.ToInt32(
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                string roleId = User.FindFirst(ClaimTypes.Role)?.Value;

                /* ================= ROLE CHECK ================= */
                // Only Admin can Delete & Archive
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
                    "[dbo].[U77_Pro_S02_StudentBatchEnrollment_Operation]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    /* ================= PARAMETERS ================= */
                    cmd.Parameters.AddWithValue("@Type", model.Type);
                    cmd.Parameters.AddWithValue("@Id", (object?)model.Id ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@StudentId", (object?)model.StudentId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BatchId", (object?)model.BatchId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@StudentRollNo", (object?)model.StudentRollNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EnrollmentId", (object?)model.EnrollmentId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UniversityEnrollNo", (object?)model.UniversityEnrollNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FormNo", (object?)model.FormNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Specialization", (object?)model.Specialization ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SectionId", (object?)model.SectionId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FeeCategoryId", (object?)model.FeeCategoryId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@GovernmentFeesDate", (object?)model.GovernmentFeesDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@GovernmentFreezeBy", (object?)model.GovernmentFreezeBy ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@GovernmentFreezeIp", (object?)model.GovernmentFreezeIp ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EnrollIpAddress", (object?)model.EnrollIpAddress ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Campaign", (object?)model.Campaign ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SubCampaign", (object?)model.SubCampaign ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DropIn", (object?)model.DropIn ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AllSemesters", (object?)model.AllSemesters ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BatchEnrollBy", (object?)model.BatchEnrollBy ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BatchEnrollDate", (object?)model.BatchEnrollDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ConfirmationDate", (object?)model.ConfirmationDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MobilizedBy", (object?)model.MobilizedBy ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsPlaced", (object?)model.IsPlaced ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AssessmentAttendance", (object?)model.AssessmentAttendance ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AssessmentResult", (object?)model.AssessmentResult ?? DBNull.Value);

                    // Logged-in user
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
