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
    public class S08StudentPreviousQualificationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public S08StudentPreviousQualificationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("operation")]
        public IActionResult StudentPreviousQualificationOperation(
            [FromBody] S08StudentPreviousQualification model)
        {
            try
            {
                /* ================= JWT CLAIMS ================= */
                int userId = Convert.ToInt32(
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                string roleId = User.FindFirst(ClaimTypes.Role)?.Value;

                /* ================= ROLE CHECK ================= */
                // Admin only: Delete & Archive
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
                    "[dbo].[U77_Pro_S08_StudentPreviousQualification_Operation]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    /* ================= PARAMETERS ================= */
                    cmd.Parameters.AddWithValue("@Type", model.Type);
                    cmd.Parameters.AddWithValue("@Id", (object?)model.Id ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@StudentId", (object?)model.StudentId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DegreeId", (object?)model.DegreeId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@QualificationId", (object?)model.QualificationId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@InstituteId", (object?)model.InstituteId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EducationBoardId", (object?)model.EducationBoardId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MediumOfInstructionId", (object?)model.MediumOfInstructionId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TCNumber", (object?)model.TCNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@RollNumber", (object?)model.RollNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PassingYearId", (object?)model.PassingYearId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TotalMarks", (object?)model.TotalMarks ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ObtainedMarks", (object?)model.ObtainedMarks ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PercentageOrCGPA", (object?)model.PercentageOrCGPA ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SectorId", (object?)model.SectorId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReasonForChange", (object?)model.ReasonForChange ?? DBNull.Value);

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
