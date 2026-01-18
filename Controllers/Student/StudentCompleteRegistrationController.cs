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
using System.Text.Json;

namespace atmglobalapi.Controllers.Student
{
    [ApiExplorerSettings(GroupName = "Student")]
    [Tags("Student")]
    [Route("api/student/")]
    [ApiController]
    [Authorize]
    public class StudentCompleteRegistrationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public StudentCompleteRegistrationController(
            IConfiguration configuration,
            IEmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
        }

        /* ===================================
           POST: COMPLETE REGISTRATION (ALL DATA)
        =================================== */
        [HttpPost("registration")]
        public async Task<IActionResult> CompleteRegistration([FromBody] StudentCompleteRegistration model)
        {
            try
            {
                /* ================= VALIDATION ================= */
                if (model.Student == null)
                    return BadRequest(new { isSuccess = false, message = "Student information is required" });

                if (string.IsNullOrEmpty(model.Student.Password))
                    return BadRequest(new { isSuccess = false, message = "Password is required" });

                if (string.IsNullOrEmpty(model.Student.PersonalEmail))
                    return BadRequest(new { isSuccess = false, message = "Personal email is required" });

                if (string.IsNullOrEmpty(model.Student.MobileNo1))
                    return BadRequest(new { isSuccess = false, message = "Mobile number is required" });

                /* ================= JWT ================= */
                int userId = Convert.ToInt32(
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                /* ================= IP ================= */
                string ipAddress =
                    HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                    ?? HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString()
                    ?? "UNKNOWN";

                /* ================= ENCRYPT PASSWORD ================= */
                CommonFunction cmf = new CommonFunction();
                string plainPassword = model.Student.Password; // Keep for email
                string encryptedPassword = cmf.Encrypt(model.Student.Password);

                /* ================= PREPARE JSON PARAMETERS ================= */
                // Student JSON with encrypted password
                var studentJson = new
                {
                    model.Student.TitleId,
                    model.Student.FirstName,
                    model.Student.MiddleName,
                    model.Student.LastName,
                    model.Student.DateOfBirth,
                    model.Student.GenderId,
                    model.Student.PersonalEmail,
                    model.Student.CollegeEmail,
                    model.Student.MobileNo1,
                    model.Student.MobileNo2,
                    model.Student.BloodGroupId,
                    model.Student.CategoryId,
                    model.Student.ReligionId,
                    model.Student.MotherTongueId,
                    model.Student.NationalityId,
                    model.Student.MaritalStatusId,
                    PasswordHash = encryptedPassword
                };

                string studentJsonStr = JsonSerializer.Serialize(studentJson);
                string enquiryJsonStr = JsonSerializer.Serialize(model.Enquiries ?? new List<EnquiryInfo>());
                string parentJsonStr = JsonSerializer.Serialize(model.Parents ?? new List<ParentInfo>());
                string addressJsonStr = JsonSerializer.Serialize(model.Addresses ?? new List<AddressInfo>());
                string academicJsonStr = JsonSerializer.Serialize(model.AcademicHistory ?? new List<AcademicInfo>());
                string schoolJsonStr = JsonSerializer.Serialize(model.LastSchool != null ? new[] { model.LastSchool } : new SchoolInfo[] { });

                DataTable dt = new DataTable();

                using (SqlConnection con =
                    new SqlConnection(_configuration.GetConnectionString("U77_Student")))
                using (SqlCommand cmd =
                    new SqlCommand("dbo.U77_Pro_StudentCompleteRegistration", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 120; // 2 minutes for complex operation

                    cmd.Parameters.AddWithValue("@StudentJson", studentJsonStr);
                    cmd.Parameters.AddWithValue("@EnquiryJson", enquiryJsonStr);
                    cmd.Parameters.AddWithValue("@ParentJson", parentJsonStr);
                    cmd.Parameters.AddWithValue("@AddressJson", addressJsonStr);
                    cmd.Parameters.AddWithValue("@AcademicJson", academicJsonStr);
                    cmd.Parameters.AddWithValue("@SchoolJson", schoolJsonStr);
                    cmd.Parameters.AddWithValue("@StudentRoleId", 5); // Student role
                    cmd.Parameters.AddWithValue("@OperationBy", userId);
                    cmd.Parameters.AddWithValue("@System", model.System ?? false);
                    cmd.Parameters.AddWithValue("@IPAddress", ipAddress);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                /* ================= SAFE RESPONSE ================= */
                if (dt.Rows.Count == 0)
                {
                    return Ok(new
                    {
                        isSuccess = false,
                        message = "Registration failed - no response from database"
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

                /* ================= SEND EMAIL (Same as S01) ================= */
                if (data.Count > 0)
                {
                    var firstRow = data[0];
                    if (firstRow.ContainsKey("isSuccess") && Convert.ToInt32(firstRow["isSuccess"]) == 1)
                    {
                        var studentName = $"{model.Student.FirstName} {model.Student.MiddleName} {model.Student.LastName}".Trim();
                        var loginId = firstRow.ContainsKey("LoginId") ? firstRow["LoginId"]?.ToString() : "";
                        var srn = firstRow.ContainsKey("SRN") ? firstRow["SRN"]?.ToString() : "";

                        // Send email asynchronously (don't wait for it to complete)
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                await _emailService.SendStudentCredentialsAsync(
                                    model.Student.PersonalEmail!,
                                    studentName,
                                    loginId ?? "",
                                    plainPassword,
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
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        /* ===================================
           POST: SHORT REGISTRATION (STUDENT + ENQUIRY ONLY)
        =================================== */
        [HttpPost("short-registration")]
        public async Task<IActionResult> ShortRegistration([FromBody] StudentShortRegistration model)
        {
            try
            {
                /* ================= VALIDATION ================= */
                if (model.Student == null)
                    return BadRequest(new { isSuccess = false, message = "Student information is required" });

                if (string.IsNullOrEmpty(model.Student.Password))
                    return BadRequest(new { isSuccess = false, message = "Password is required" });

                if (string.IsNullOrEmpty(model.Student.PersonalEmail))
                    return BadRequest(new { isSuccess = false, message = "Personal email is required" });

                if (string.IsNullOrEmpty(model.Student.MobileNo1))
                    return BadRequest(new { isSuccess = false, message = "Mobile number is required" });

                /* ================= JWT ================= */
                int userId = Convert.ToInt32(
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                /* ================= IP ================= */
                string ipAddress =
                    HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                    ?? HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString()
                    ?? "UNKNOWN";

                /* ================= ENCRYPT PASSWORD ================= */
                CommonFunction cmf = new CommonFunction();
                string plainPassword = model.Student.Password; // Keep for email
                string encryptedPassword = cmf.Encrypt(model.Student.Password);

                /* ================= PREPARE JSON PARAMETERS ================= */
                var studentJson = new
                {
                    model.Student.TitleId,
                    model.Student.FirstName,
                    model.Student.MiddleName,
                    model.Student.LastName,
                    model.Student.DateOfBirth,
                    model.Student.GenderId,
                    model.Student.PersonalEmail,
                    model.Student.CollegeEmail,
                    model.Student.MobileNo1,
                    model.Student.MobileNo2,
                    model.Student.BloodGroupId,
                    model.Student.CategoryId,
                    model.Student.ReligionId,
                    model.Student.MotherTongueId,
                    model.Student.NationalityId,
                    model.Student.MaritalStatusId,
                    PasswordHash = encryptedPassword
                };

                string studentJsonStr = JsonSerializer.Serialize(studentJson);
                string enquiryJsonStr = JsonSerializer.Serialize(model.Enquiries ?? new List<EnquiryInfo>());

                DataTable dt = new DataTable();

                using (SqlConnection con =
                    new SqlConnection(_configuration.GetConnectionString("U77_Student")))
                using (SqlCommand cmd =
                    new SqlCommand("dbo.U77_Pro_StudentShortRegistration", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 60;

                    cmd.Parameters.AddWithValue("@StudentJson", studentJsonStr);
                    cmd.Parameters.AddWithValue("@EnquiryJson", enquiryJsonStr);
                    cmd.Parameters.AddWithValue("@StudentRoleId", 5); // Student role
                    cmd.Parameters.AddWithValue("@OperationBy", userId);
                    cmd.Parameters.AddWithValue("@System", model.System ?? false);
                    cmd.Parameters.AddWithValue("@IPAddress", ipAddress);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                /* ================= SAFE RESPONSE ================= */
                if (dt.Rows.Count == 0)
                {
                    return Ok(new
                    {
                        isSuccess = false,
                        message = "Registration failed - no response from database"
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

                /* ================= SEND EMAIL ================= */
                if (data.Count > 0)
                {
                    var firstRow = data[0];
                    if (firstRow.ContainsKey("isSuccess") && Convert.ToInt32(firstRow["isSuccess"]) == 1)
                    {
                        var studentName = $"{model.Student.FirstName} {model.Student.MiddleName} {model.Student.LastName}".Trim();
                        var loginId = firstRow.ContainsKey("LoginId") ? firstRow["LoginId"]?.ToString() : "";
                        var srn = firstRow.ContainsKey("SRN") ? firstRow["SRN"]?.ToString() : "";

                        // Send email asynchronously
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                await _emailService.SendStudentCredentialsAsync(
                                    model.Student.PersonalEmail!,
                                    studentName,
                                    loginId ?? "",
                                    plainPassword,
                                    srn ?? ""
                                );
                            }
                            catch (Exception emailEx)
                            {
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
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        /* ===================================
           GET: COMPLETE STUDENT PROFILE
        =================================== */
        [HttpGet("profile/{studentId}")]
        public IActionResult GetCompleteProfile(long studentId)
        {
            try
            {
                if (studentId <= 0)
                    return BadRequest(new { isSuccess = false, message = "Valid StudentId is required" });

                DataTable dt = new DataTable();

                using (SqlConnection con =
                    new SqlConnection(_configuration.GetConnectionString("U77_Student")))
                using (SqlCommand cmd =
                    new SqlCommand("dbo.U77_Pro_GetStudentCompleteProfile", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 60;

                    cmd.Parameters.AddWithValue("@StudentId", studentId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                /* ================= PARSE JSON RESPONSE ================= */
                if (dt.Rows.Count == 0)
                {
                    return Ok(new
                    {
                        isSuccess = false,
                        message = "Student not found"
                    });
                }

                // The stored procedure returns JSON, so we need to extract it
                var jsonResult = dt.Rows[0][0]?.ToString();

                if (string.IsNullOrEmpty(jsonResult))
                {
                    return Ok(new
                    {
                        isSuccess = false,
                        message = "No data returned from database"
                    });
                }

                // Parse the JSON and return it as an object
                var profileData = JsonSerializer.Deserialize<object>(jsonResult);

                return Ok(profileData);
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

        /* ===================================
           GET: COMPLETE PROFILE (QUERY PARAM)
        =================================== */
        [HttpGet("profile")]
        public IActionResult GetCompleteProfileByQuery([FromQuery] long studentId)
        {
            return GetCompleteProfile(studentId);
        }
    }

    /* ===================================
       MODEL: SHORT REGISTRATION
    =================================== */
    public class StudentShortRegistration
    {
        public StudentInfo? Student { get; set; }
        public List<EnquiryInfo>? Enquiries { get; set; }
        public bool? System { get; set; }
    }
}