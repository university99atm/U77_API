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
    public class A04RoleMenuPermissionController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public A04RoleMenuPermissionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /* ===================================
           ASSIGN ROLE MENU PERMISSIONS
        =================================== */
        [HttpPost("assign")]
        public IActionResult AssignRoleMenus([FromBody] A04RoleMenuPermission model)
        {
            try
            {
                /* ================= JWT ================= */
                int userId = Convert.ToInt32(
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                string roleId =
                    User.FindFirst(ClaimTypes.Role)?.Value ?? "0";

                // Only admin can assign role permissions
                //if (roleId != "1")
                //{
                //    return Unauthorized(new
                //    {
                //        isSuccess = false,
                //        message = "You are not authorized"
                //    });
                //}

                /* ================= IP ================= */
                string ipAddress =
                    HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                    ?? HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString()
                    ?? "UNKNOWN";

                /* ================= CREATE TVP ================= */
                DataTable tvpMenuList = new DataTable();
                tvpMenuList.Columns.Add("MenuId", typeof(int));
                tvpMenuList.Columns.Add("CanView", typeof(bool));
                tvpMenuList.Columns.Add("CanAdd", typeof(bool));
                tvpMenuList.Columns.Add("CanEdit", typeof(bool));
                tvpMenuList.Columns.Add("CanDelete", typeof(bool));
                tvpMenuList.Columns.Add("CanApprove", typeof(bool));

                if (model.MenuList != null)
                {
                    foreach (var menu in model.MenuList)
                    {
                        tvpMenuList.Rows.Add(
                            menu.MenuId,
                            menu.CanView,
                            menu.CanAdd,
                            menu.CanEdit,
                            menu.CanDelete,
                            menu.CanApprove
                        );
                    }
                }

                DataTable dt = new DataTable();

                using (SqlConnection con =
                    new SqlConnection(_configuration.GetConnectionString("U77_User")))
                using (SqlCommand cmd =
                    new SqlCommand("dbo.U77_Pro_A04_AssignRoleMenus", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Type", model.Type);
                    cmd.Parameters.AddWithValue("@RoleId", model.RoleId);

                    // ✅ Add TVP parameter
                    var tvpParam = cmd.Parameters.AddWithValue("@MenuList", tvpMenuList);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    tvpParam.TypeName = "dbo.TVP_RoleMenuPermission";

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

        /* ===================================
           GET ROLE MENU PERMISSIONS
        =================================== */
        [HttpGet("role/{roleId}")]
        public IActionResult GetRoleMenuPermissions(int roleId)
        {
            try
            {
                DataTable dt = new DataTable();

                using (SqlConnection con =
                    new SqlConnection(_configuration.GetConnectionString("U77_User")))
                using (SqlCommand cmd =
                    new SqlCommand(@"
                        SELECT 
                            p.RoleId,
                            p.MenuId,
                            m.MenuName,
                            m.MenuCode,
                            m.ParentMenuId,
                            p.CanView,
                            p.CanAdd,
                            p.CanEdit,
                            p.CanDelete,
                            p.CanApprove,
                            p.Status
                        FROM dbo.U77_A04_RoleMenuPermission p
                        INNER JOIN dbo.U77_A03_Menu m ON p.MenuId = m.MenuId
                        WHERE p.RoleId = @RoleId
                          AND p.Status = 1
                        ORDER BY m.ParentMenuId, m.DisplayOrder
                    ", con))
                {
                    cmd.Parameters.AddWithValue("@RoleId", roleId);

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