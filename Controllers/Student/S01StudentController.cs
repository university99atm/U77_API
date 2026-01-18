using atmglobalapi.Model.Student;
using atmglobalapi.Services;
using CommonClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;

namespace atmglobalapi.Controllers.Student
{
    [ApiExplorerSettings(GroupName = "Student")]
    [Tags("Student")]
    [Route("api/student/[controller]")]
    [ApiController]
    [Authorize]
    public class S01StudentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public S01StudentController(IConfiguration configuration, IEmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost("operation")]
        public async Task<IActionResult> StudentOperation([FromBody] S01Student model)
        {
            try
            {
                /* ================= JWT ================= */
                int userId = Convert.ToInt32(
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                string roleId =
                    User.FindFirst(ClaimTypes.Role)?.Value ?? "0";

                /* ================= ROLE CHECK ================= */
                // Admin only for Delete & View Deleted
                if ((model.Type == 3 || model.Type == 8) && roleId != "1")
                {
                    return Unauthorized(new
                    {
                        isSuccess = false,
                        message = "You are not authorized"
                    });
                }

                /* ================= IP ================= */
                string ipAddress =
                    HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                    ?? HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString()
                    ?? "UNKNOWN";

                /* ================= ENCRYPT PASSWORD (Type 1 Only) ================= */
                string? encryptedPassword = null;
                string? plainPassword = null; // Keep for email
                if (model.Type == 1 && !string.IsNullOrEmpty(model.Password))
                {
                    CommonFunction cmf = new CommonFunction();
                    plainPassword = model.Password; // Store original password for email
                    encryptedPassword = cmf.Encrypt(model.Password);
                }

                DataTable dt = new DataTable();

                using (SqlConnection con =
                    new SqlConnection(_configuration.GetConnectionString("U77_Student")))
                using (SqlCommand cmd =
                    new SqlCommand("dbo.U77_Pro_S01_StudentOperation", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Type", model.Type);
                    cmd.Parameters.AddWithValue("@Id", (object?)model.Id ?? DBNull.Value);

                    // Personal Info
                    cmd.Parameters.AddWithValue("@TitleId", (object?)model.TitleId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FirstName", (object?)model.FirstName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MiddleName", (object?)model.MiddleName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastName", (object?)model.LastName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateOfBirth", (object?)model.DateOfBirth ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@GenderId", (object?)model.GenderId ?? DBNull.Value);

                    // Contact Info
                    cmd.Parameters.AddWithValue("@PersonalEmail", (object?)model.PersonalEmail ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CollegeEmail", (object?)model.CollegeEmail ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MobileNo1", (object?)model.MobileNo1 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MobileNo2", (object?)model.MobileNo2 ?? DBNull.Value);

                    // Additional Info
                    cmd.Parameters.AddWithValue("@BloodGroupId", (object?)model.BloodGroupId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CategoryId", (object?)model.CategoryId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReligionId", (object?)model.ReligionId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MotherTongueId", (object?)model.MotherTongueId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NationalityId", (object?)model.NationalityId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaritalStatusId", (object?)model.MaritalStatusId ?? DBNull.Value);

                    // Status
                    cmd.Parameters.AddWithValue("@Status", (object?)model.Status ?? DBNull.Value);

                    // Pagination
                    cmd.Parameters.AddWithValue("@PageNumber", model.PageNumber ?? 1);
                    cmd.Parameters.AddWithValue("@PageSize", model.PageSize ?? 10);
                    cmd.Parameters.AddWithValue("@Search", (object?)model.Search ?? DBNull.Value);

                    // Audit
                    cmd.Parameters.AddWithValue("@System", model.System ?? false);
                    cmd.Parameters.AddWithValue("@IPAddress", ipAddress);
                    cmd.Parameters.AddWithValue("@OperationBy", userId);

                    // Encrypted Password (for Type 1)
                    cmd.Parameters.AddWithValue("@PasswordHash", (object?)encryptedPassword ?? DBNull.Value);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                /* ================= SAFE RESPONSE ================= */
                if (dt.Rows.Count == 0)
                {
                    return Ok(new
                    {
                        isSuccess = true,
                        data = new List<object>()
                    });
                }

                var data = dt.AsEnumerable()
                    .Select(row =>
                    {
                        var dict = new Dictionary<string, object?>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            dict[col.ColumnName] =
                                row[col] == DBNull.Value ? null : row[col];
                        }
                        return dict;
                    })
                    .ToList();

                /* ================= SEND EMAIL (Type 1 Only - Registration) ================= */
                if (model.Type == 1 && data.Count > 0 && !string.IsNullOrEmpty(model.PersonalEmail))
                {
                    var firstRow = data[0];
                    if (firstRow.ContainsKey("isSuccess") && Convert.ToInt32(firstRow["isSuccess"]) == 1)
                    {
                        var studentName = $"{model.FirstName} {model.MiddleName} {model.LastName}".Trim();
                        var loginId = firstRow.ContainsKey("LoginId") ? firstRow["LoginId"]?.ToString() : "";
                        var srn = firstRow.ContainsKey("SRN") ? firstRow["SRN"]?.ToString() : "";

                        // Send email asynchronously (don't wait for it to complete)
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                await _emailService.SendStudentCredentialsAsync(
                                    model.PersonalEmail,
                                    studentName,
                                    loginId ?? "",
                                    plainPassword ?? "",
                                    srn ?? ""
                                );
                            }
                            catch (Exception emailEx)
                            {
                                // Log but don't fail the registration
                                Console.WriteLine($"Email send failed: {emailEx.Message}");
                            }
                        });
                    }
                }

                return Ok(new
                {
                    isSuccess = true,
                    data
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