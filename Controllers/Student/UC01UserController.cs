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
    public class UC01UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UC01UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("operation")]
        public IActionResult UserOperation([FromBody] UC01User model)
        {
            try
            {
                /* ================= JWT ================= */
                int actionBy = Convert.ToInt32(
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                string roleId = User.FindFirst(ClaimTypes.Role)?.Value;

                /* ================= ROLE SECURITY ================= */
                // Delete, Archive, Lock/Unlock → Admin only
                if ((model.Type == 3 || model.Type == 4 || model.Type == 6) && roleId != "1")
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
                    "[dbo].[U77_Pro_UC01_User_Operation]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    /* ================= PARAMETERS ================= */
                    cmd.Parameters.AddWithValue("@Type", model.Type);
                    cmd.Parameters.AddWithValue("@Id", (object?)model.Id ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@UserName", (object?)model.UserName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PasswordHash", (object?)model.PasswordHash ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SRNNo", (object?)model.SRNNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserID", (object?)model.UserID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@RollId", (object?)model.RollId ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@IsLocked", (object?)model.IsLocked ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastLogin", (object?)model.LastLogin ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", (object?)model.Status ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Archive", (object?)model.Archive ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsActive", (object?)model.IsActive ?? DBNull.Value);

                    // JWT user
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
