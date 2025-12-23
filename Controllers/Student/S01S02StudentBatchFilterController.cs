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
    public class S01S02StudentBatchFilterController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public S01S02StudentBatchFilterController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("filter")]
        public IActionResult FilterStudentBatch([FromBody] S01S02StudentBatchFilter model)
        {
            try
            {
                /* ================= JWT ================= */
                int userId = Convert.ToInt32(
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("U77_Common")))
                using (SqlCommand cmd = new SqlCommand(
                    "[dbo].[U77_Pro_S01_S02_Filter_WithPagination]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    /* ================= STUDENT FILTERS ================= */
                    cmd.Parameters.AddWithValue("@StudentId", (object?)model.StudentId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SRN", (object?)model.SRN ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@StudentName", (object?)model.StudentName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NationalityId", (object?)model.NationalityId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReligionId", (object?)model.ReligionId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CategoryId", (object?)model.CategoryId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Gender", (object?)model.Gender ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaritalStatus", (object?)model.MaritalStatus ?? DBNull.Value);

                    /* ================= ENROLLMENT FILTERS ================= */
                    cmd.Parameters.AddWithValue("@SectionId", (object?)model.SectionId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FeeCategoryId", (object?)model.FeeCategoryId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EnrollmentId", (object?)model.EnrollmentId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FromDate", (object?)model.FromDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ToDate", (object?)model.ToDate ?? DBNull.Value);

                    /* ================= BATCH FILTERS ================= */
                    cmd.Parameters.AddWithValue("@OrganizationId", (object?)model.OrganizationId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CollegeId", (object?)model.CollegeId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BranchId", (object?)model.BranchId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CourseTypeId", (object?)model.CourseTypeId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UniversityId", (object?)model.UniversityId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SectorId", (object?)model.SectorId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CourseId", (object?)model.CourseId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BatchId", (object?)model.BatchId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BatchCode", (object?)model.BatchCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BatchName", (object?)model.BatchName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BatchType", (object?)model.BatchType ?? DBNull.Value);

                    /* ================= PAGINATION ================= */
                    cmd.Parameters.AddWithValue("@PageNo", model.PageNo);
                    cmd.Parameters.AddWithValue("@PageSize", model.PageSize);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                /* ================= RESPONSE ================= */
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

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
                    message = "No data returned from database",
                    data = "null"
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
