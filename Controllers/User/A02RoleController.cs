using atmglobalapi.Model.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;

namespace atmglobalapi.Controllers.User
{
    [ApiExplorerSettings(GroupName = "User")]
    [Tags("User")]
    [Route("api/user/[controller]")]
    [ApiController]
    [Authorize]
    public class A02RoleController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public A02RoleController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("operation")]
        public IActionResult RoleOperation([FromBody] A02Role model)
        {
            try
            {
                /* ================= JWT ================= */
                int userId = Convert.ToInt32(
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                string roleId =
                    User.FindFirst(ClaimTypes.Role)?.Value ?? "0";

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
                    new SqlConnection(_configuration.GetConnectionString("U77_User")))
                using (SqlCommand cmd =
                    new SqlCommand("dbo.U77_Pro_A02_RoleOperation", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Type", model.Type);
                    cmd.Parameters.AddWithValue("@RoleId", (object?)model.RoleId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@RoleName", (object?)model.RoleName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Description", (object?)model.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", (object?)model.Status ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@PageNumber", model.PageNumber ?? 1);
                    cmd.Parameters.AddWithValue("@PageSize", model.PageSize ?? 10);
                    cmd.Parameters.AddWithValue("@Search", (object?)model.Search ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@IsSystem", model.IsSystem ?? false);
                    cmd.Parameters.AddWithValue("@IPAddress", ipAddress);
                    cmd.Parameters.AddWithValue("@ActionBy", userId);

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