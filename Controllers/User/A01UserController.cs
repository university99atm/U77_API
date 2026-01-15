using atmglobalapi.Model.User;
using atmglobalapi.Services;  // ✅ This line should already be there
using CommonClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System;

namespace atmglobalapi.Controllers.User
{
    [ApiExplorerSettings(GroupName = "User")]
    [Tags("User")]
    [Route("api/user/[controller]")]
    [ApiController]
    public class A01UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IJwtService _jwtService;  // ✅ Added JWT service

        public A01UserController(IConfiguration configuration, IJwtService jwtService)
        {
            _configuration = configuration;
            _jwtService = jwtService;
        }

        /* ===================================
           REGISTER USER
        =================================== */
        [HttpPost("register")]
        [Authorize]
        public IActionResult Register([FromBody] A01User model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Password))
                    return BadRequest(new { isSuccess = false, message = "Password cannot be empty" });

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
                string encryptedPassword = cmf.Encrypt(model.Password);

                DataTable dt = new DataTable();

                using (SqlConnection con =
                    new SqlConnection(_configuration.GetConnectionString("U77_User")))
                using (SqlCommand cmd =
                    new SqlCommand("dbo.U77_Pro_A01_UserRegister", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@UserName", (object?)model.UserName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LoginId", (object?)model.LoginId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PasswordHash", encryptedPassword);  // ✅ Encrypted
                    cmd.Parameters.AddWithValue("@Email", (object?)model.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MobileNo", (object?)model.MobileNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@RoleIds", (object?)model.RoleIds ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@OperationBy", userId);
                    cmd.Parameters.AddWithValue("@System", model.IsSystem ?? false);
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
           LOGIN USER
        =================================== */
        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] A01User model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.LoginId) || string.IsNullOrEmpty(model.Password))
                    return BadRequest(new { isSuccess = false, message = "LoginId or Password cannot be empty" });

                /* ================= IP ================= */
                string ipAddress =
                    HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                    ?? HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString()
                    ?? "UNKNOWN";

                /* ================= ENCRYPT PASSWORD ================= */
                CommonFunction cmf = new CommonFunction();
                string encryptedPassword = cmf.Encrypt(model.Password);

                DataSet ds = new DataSet();

                using (SqlConnection con =
                    new SqlConnection(_configuration.GetConnectionString("U77_User")))
                using (SqlCommand cmd =
                    new SqlCommand("dbo.U77_Pro_A01_UserLogin", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@LoginId", model.LoginId);
                    cmd.Parameters.AddWithValue("@PasswordHash", encryptedPassword);  // ✅ Encrypted
                    cmd.Parameters.AddWithValue("@IPAddress", ipAddress);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                }

                /* ================= PROCESS LOGIN RESPONSE ================= */
                if (ds.Tables.Count == 0)
                {
                    return Ok(new
                    {
                        isSuccess = false,
                        message = "Invalid credentials"
                    });
                }

                // Table 0: User Basic Info
                var userTable = ds.Tables[0];
                if (userTable.Rows.Count == 0)
                {
                    return Ok(new
                    {
                        isSuccess = false,
                        message = "Invalid credentials or user inactive"
                    });
                }

                var userRow = userTable.Rows[0];
                var userId = userRow["UserId"].ToString();
                var userName = userRow["UserName"].ToString();
                var email = userRow["Email"].ToString();

                // Table 1: User Roles
                var rolesTable = ds.Tables.Count > 1 ? ds.Tables[1] : null;
                var roles = new List<object>();
                if (rolesTable != null)
                {
                    roles = rolesTable.AsEnumerable()
                        .Select(row => new
                        {
                            RoleId = row["RoleId"],
                            RoleName = row["RoleName"],
                            RoleCode = row["RoleCode"]
                        })
                        .ToList<object>();
                }

                // Table 2: Menu + Permissions
                var menuTable = ds.Tables.Count > 2 ? ds.Tables[2] : null;
                var menus = new List<object>();
                if (menuTable != null)
                {
                    menus = menuTable.AsEnumerable()
                        .Select(row => new
                        {
                            MenuId = row["MenuId"],
                            MenuName = row["MenuName"],
                            ParentMenuId = row["ParentMenuId"] == DBNull.Value ? null : row["ParentMenuId"],
                            MenuUrl = row["MenuUrl"],
                            MenuIcon = row["MenuIcon"],
                            CanView = row["CanView"],
                            CanAdd = row["CanAdd"],
                            CanEdit = row["CanEdit"],
                            CanDelete = row["CanDelete"]
                        })
                        .ToList<object>();
                }

                /* ================= GENERATE JWT TOKEN ================= */
                var primaryRole = roles.Count > 0
                    ? ((dynamic)roles[0]).RoleId.ToString()
                    : "0";

                var token = _jwtService.GenerateToken(userId, email ?? "", primaryRole);

                return Ok(new
                {
                    isSuccess = true,
                    message = "Login successful",
                    data = new
                    {
                        Token = token,
                        User = new
                        {
                            UserId = userId,
                            UserName = userName,
                            LoginId = model.LoginId,
                            Email = email,
                            LastLoginAt = userRow["LastLoginAt"]
                        },
                        Roles = roles,
                        Menus = menus
                    }
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
           UPDATE PASSWORD
        =================================== */
        [HttpPost("update-password")]
        [Authorize]
        public IActionResult UpdatePassword([FromBody] A01User model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.NewPassword))
                    return BadRequest(new { isSuccess = false, message = "New password cannot be empty" });

                /* ================= JWT ================= */
                int userId = Convert.ToInt32(
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                /* ================= IP ================= */
                string ipAddress =
                    HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                    ?? HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString()
                    ?? "UNKNOWN";

                /* ================= ENCRYPT PASSWORDS ================= */
                CommonFunction cmf = new CommonFunction();
                string? encryptedOldPassword = null;
                if (!string.IsNullOrEmpty(model.OldPassword))
                {
                    encryptedOldPassword = cmf.Encrypt(model.OldPassword);
                }
                string encryptedNewPassword = cmf.Encrypt(model.NewPassword);

                DataTable dt = new DataTable();

                using (SqlConnection con =
                    new SqlConnection(_configuration.GetConnectionString("U77_User")))
                using (SqlCommand cmd =
                    new SqlCommand("dbo.U77_Pro_A01_UpdatePassword", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Type", model.Type ?? 1);
                    cmd.Parameters.AddWithValue("@UserId", model.UserId ?? userId);
                    cmd.Parameters.AddWithValue("@OldPasswordHash", (object?)encryptedOldPassword ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NewPasswordHash", encryptedNewPassword);

                    cmd.Parameters.AddWithValue("@ActionBy", userId);
                    cmd.Parameters.AddWithValue("@IPAddress", ipAddress);
                    cmd.Parameters.AddWithValue("@IsSystem", model.IsSystem ?? false);

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
