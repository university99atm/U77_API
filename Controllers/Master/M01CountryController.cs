using atmglobalapi.Model.Master;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Linq;
using System.Security.Claims;

namespace atmglobalapi.Controllers.Master
{
    [ApiExplorerSettings(GroupName = "Master")]
    [Tags("Master")]
    [Route("api/master/[controller]")]
    [ApiController]
    [Authorize]
    public class M01CountryController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public M01CountryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("operation")]
        public IActionResult CountryOperation([FromBody] M01Country model)
        {
            try
            {
                /* ================= JWT CLAIMS ================= */
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
                string roleId = User.FindFirst(ClaimTypes.Role)?.Value ?? "0";

                /* ================= ROLE CHECK ================= */
                if ((model.Type == 3 || model.Type == 4) && roleId != "1")
                {
                    return Unauthorized(new
                    {
                        isSuccess = false,
                        message = "You are not authorized to perform this action"
                    });
                }

                /* ================= CLIENT IP ================= */
                string ipAddress =
                    HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                    ?? HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString()
                    ?? "UNKNOWN";


                DataTable dt = new DataTable();

                using (SqlConnection con =
                    new SqlConnection(_configuration.GetConnectionString("U77_Common")))
                using (SqlCommand cmd =
                    new SqlCommand("[dbo].[U77_Pro_M01_countryoperation]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Type", model.Type);
                    cmd.Parameters.AddWithValue("@Id", (object?)model.Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CountryName", (object?)model.CountryName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@status", (object?)model.status ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@archive", (object?)model.archive ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Country_Code", (object?)model.Country_Code ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@is_active", (object?)model.is_active ?? DBNull.Value);

                    // 🔹 NEW FIELDS
                    cmd.Parameters.AddWithValue("@System", (object?)model.System ?? 0);
                    cmd.Parameters.AddWithValue("@IPAddress", ipAddress);

                    cmd.Parameters.AddWithValue("@operation_by", Convert.ToInt32(userId));

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
                                row["data"].ToString()!)
                    });
                }

                return Ok(new
                {
                    isSuccess = false,
                    message = "No response from database",
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
