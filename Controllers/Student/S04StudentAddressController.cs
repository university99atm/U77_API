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
    public class S04StudentAddressController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public S04StudentAddressController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("operation")]
        public IActionResult StudentAddressOperation([FromBody] S04StudentAddress model)
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
                    new SqlCommand("dbo.U77_Pro_S04_Student_AddressOperation", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Type", model.Type);
                    cmd.Parameters.AddWithValue("@Id", (object?)model.Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@StudentId", (object?)model.StudentId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AddressTypeId", (object?)model.AddressTypeId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AddressLine1", (object?)model.AddressLine1 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AddressLine2", (object?)model.AddressLine2 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AreaId", (object?)model.AreaId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CityId", (object?)model.CityId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DistrictId", (object?)model.DistrictId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@StateId", (object?)model.StateId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CountryId", (object?)model.CountryId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Pincode", (object?)model.Pincode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsCurrent", (object?)model.IsCurrent ?? DBNull.Value);
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