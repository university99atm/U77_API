using atmglobalapi.Model.Lead;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace atmglobalapi.Controllers.Lead
{
    [ApiExplorerSettings(GroupName = "Lead")]
    [Tags("Lead")]
    [Route("api/lead/")]
    [ApiController]
    [Authorize]
    public class L01LeadController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public L01LeadController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /* ===================================
           POST: COMPLETE LEAD REGISTRATION
        =================================== */
        [HttpPost("registration")]
        public IActionResult CompleteLeadRegistration([FromBody] LeadCompleteRegistration model)
        {
            try
            {
                /* ================= VALIDATION ================= */
                if (model.Lead == null)
                    return BadRequest(new { isSuccess = false, message = "Lead information is required" });

                if (string.IsNullOrEmpty(model.Lead.FirstName))
                    return BadRequest(new { isSuccess = false, message = "First name is required" });

                if (string.IsNullOrEmpty(model.Lead.MobileNo1))
                    return BadRequest(new { isSuccess = false, message = "Mobile number is required" });

                /* ================= JWT ================= */
                int userId = Convert.ToInt32(
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                /* ================= IP ================= */
                string ipAddress =
                    HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                    ?? HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString()
                    ?? "UNKNOWN";

                /* ================= PREPARE JSON PARAMETERS ================= */
                string leadJsonStr = JsonSerializer.Serialize(model.Lead);
                string courseJsonStr = JsonSerializer.Serialize(model.Courses ?? new List<LeadCourseInfo>());

                DataTable dt = new DataTable();

                using (SqlConnection con =
                    new SqlConnection(_configuration.GetConnectionString("U77_Student")))
                using (SqlCommand cmd =
                    new SqlCommand("dbo.U77_Pro_L01_LeadOperation", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 60;

                    cmd.Parameters.AddWithValue("@Type", 1); // Type 1 = Complete Registration
                    cmd.Parameters.AddWithValue("@LeadJson", leadJsonStr);
                    cmd.Parameters.AddWithValue("@CourseJson", courseJsonStr);
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
           POST: LEAD OPERATION (Types 2-7)
        =================================== */
        [HttpPost("operation")]
        public IActionResult LeadOperation([FromBody] L01Lead model)
        {
            try
            {
                /* ================= JWT ================= */
                int userId = Convert.ToInt32(
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                string roleId =
                    User.FindFirst(ClaimTypes.Role)?.Value ?? "0";

                /* ================= ROLE CHECK ================= */
                if (model.Type == 3 && roleId != "1")
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

                DataTable dt = new DataTable();

                using (SqlConnection con =
                    new SqlConnection(_configuration.GetConnectionString("U77_Student")))
                using (SqlCommand cmd =
                    new SqlCommand("dbo.U77_Pro_L01_LeadOperation", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Type", model.Type);
                    cmd.Parameters.AddWithValue("@Id", (object?)model.Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", (object?)model.Status ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PageNumber", model.PageNumber ?? 1);
                    cmd.Parameters.AddWithValue("@PageSize", model.PageSize ?? 10);
                    cmd.Parameters.AddWithValue("@Search", (object?)model.Search ?? DBNull.Value);
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

        /* ===================================
           GET: COMPLETE LEAD PROFILE
        =================================== */
        [HttpGet("profile/{leadId}")]
        public IActionResult GetCompleteProfile(long leadId)
        {
            try
            {
                if (leadId <= 0)
                    return BadRequest(new { isSuccess = false, message = "Valid LeadId is required" });

                string jsonResult = string.Empty;

                using (SqlConnection con =
                    new SqlConnection(_configuration.GetConnectionString("U77_Student")))
                using (SqlCommand cmd =
                    new SqlCommand("dbo.U77_Pro_L01_LeadOperation", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 60;

                    cmd.Parameters.AddWithValue("@Type", 10); // Type 10 = Get Complete Profile
                    cmd.Parameters.AddWithValue("@Id", leadId);

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            jsonResult = reader.GetString(0);
                        }
                    }
                }

                if (string.IsNullOrEmpty(jsonResult))
                {
                    return NotFound(new
                    {
                        isSuccess = false,
                        message = "Lead not found"
                    });
                }

                // Return raw JSON content directly
                return Content(jsonResult, "application/json");
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
        public IActionResult GetCompleteProfileByQuery([FromQuery] long leadId)
        {
            return GetCompleteProfile(leadId);
        }
    }
}