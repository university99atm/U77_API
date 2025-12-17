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
    public class S01StudentEnrollmentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public S01StudentEnrollmentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("operation")]
        public IActionResult StudentEnrollmentOperation([FromBody] S01StudentEnrollment model)
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
                    "[dbo].[U77_Pro_S01_StudentEnrol_WithUser_Operation]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    /* ================= PARAMETERS ================= */
                    cmd.Parameters.AddWithValue("@Type", model.Type);
                    cmd.Parameters.AddWithValue("@StudentId", (object?)model.StudentId ?? DBNull.Value);

                    // Student
                    cmd.Parameters.AddWithValue("@SRN", (object?)model.SRN ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@StudentCode", (object?)model.StudentCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TitleId", (object?)model.TitleId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FirstName", (object?)model.FirstName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MiddleName", (object?)model.MiddleName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastName", (object?)model.LastName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateOfBirth", (object?)model.DateOfBirth ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Gender", (object?)model.Gender ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PersonalEmail", (object?)model.PersonalEmail ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CollegeEmail", (object?)model.CollegeEmail ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MobileNo1", (object?)model.MobileNo1 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MobileNo2", (object?)model.MobileNo2 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BloodGroup", (object?)model.BloodGroup ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CategoryId", (object?)model.CategoryId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@HighestQualificationId", (object?)model.HighestQualificationId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaritalStatus", (object?)model.MaritalStatus ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NationalityId", (object?)model.NationalityId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MotherTongueId", (object?)model.MotherTongueId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReligionId", (object?)model.ReligionId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FinancialYearId", (object?)model.FinancialYearId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@StatusId", (object?)model.StatusId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Remarks", (object?)model.Remarks ?? DBNull.Value);

                    // User
                    cmd.Parameters.AddWithValue("@UserName", (object?)model.UserName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PasswordHash", (object?)model.PasswordHash ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@RollId", (object?)model.RollId ?? DBNull.Value);

                    // JWT user
                    cmd.Parameters.AddWithValue("@ActionBy", userId);

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
