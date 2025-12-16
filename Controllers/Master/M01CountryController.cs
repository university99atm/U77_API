using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using atmglobalapi.Model.Master;

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
                // 🔐 Extract claims from JWT
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string roleId = User.FindFirst(ClaimTypes.Role)?.Value;

                // 🔒 Internal authorization
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
                    "[dbo].[U77_Pro_M01_countryoperation]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Type", model.Type);
                    cmd.Parameters.AddWithValue("@Id", (object?)model.Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CountryName", (object?)model.CountryName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@status", (object?)model.status ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@archive", (object?)model.archive ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Country_Code", (object?)model.Country_Code ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@is_active", (object?)model.is_active ?? DBNull.Value);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                // ✅ Safely return SP JSON output
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
