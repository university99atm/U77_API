using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using atmglobalapi.Model.User;

namespace atmglobalapi.Controllers.User
{
    [ApiExplorerSettings(GroupName = "User")]
    [Tags("User")]
    [Route("api/Student/[controller]")]
    [ApiController]
    [Authorize]
    public class UC02RoleMenuPermissionController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UC02RoleMenuPermissionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("operation")]
        public IActionResult RoleMenuPermissionOperation([FromBody] UC02RoleMenuPermission model)
        {
            try
            {
                /* ================= JWT ================= */
                int actionBy = Convert.ToInt32(
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                string roleId = User.FindFirst(ClaimTypes.Role)?.Value;

                /* ================= ROLE SECURITY ================= */
                // Only Admin can Delete / Archive
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
                    "[dbo].[U77_Pro_UC02_RoleMenuPermission_Operation]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    /* ================= PARAMETERS ================= */
                    cmd.Parameters.AddWithValue("@Type", model.Type);
                    cmd.Parameters.AddWithValue("@Id", (object?)model.Id ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@RoleId", (object?)model.RoleId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MenuId", (object?)model.MenuId ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@CanView", (object?)model.CanView ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CanAdd", (object?)model.CanAdd ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CanEdit", (object?)model.CanEdit ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CanDelete", (object?)model.CanDelete ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CanApprove", (object?)model.CanApprove ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@Status", (object?)model.Status ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Archive", (object?)model.Archive ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@ActionBy", actionBy);

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
