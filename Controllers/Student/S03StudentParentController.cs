using atmglobalapi.Model.Student;
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
    public class S03StudentParentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public S03StudentParentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("operation")]
        public IActionResult StudentParentOperation([FromBody] S03StudentParent model)
        {
            try
            {
                /* ================= JWT ================= */
                int userId = Convert.ToInt32(
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                string roleId =
                    User.FindFirst(ClaimTypes.Role)?.Value ?? "0";

                /* ================= ROLE CHECK ================= */
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

                DataTable dt = new DataTable();

                using (SqlConnection con =
                    new SqlConnection(_configuration.GetConnectionString("U77_Student")))
                using (SqlCommand cmd =
                    new SqlCommand("dbo.U77_Pro_S03_Student_ParentOperation", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

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
                    cmd.Parameters.AddWithValue("@AnnualIncomeId", (object?)model.AnnualIncomeId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ExactIncome", (object?)model.ExactIncome ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", (object?)model.Status ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PageNumber", model.PageNumber ?? 1);
                    cmd.Parameters.AddWithValue("@PageSize", model.PageSize ?? 10);
                    cmd.Parameters.AddWithValue("@Search", (object?)model.Search ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@System", model.System ?? false);
                    cmd.Parameters.AddWithValue("@IPAddress", ipAddress);
                    cmd.Parameters.AddWithValue("@OperationBy", userId);

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
    }
}