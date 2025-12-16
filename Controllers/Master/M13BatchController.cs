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
    public class M13BatchController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public M13BatchController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("operation")]
        public IActionResult BatchOperation([FromBody] M13Batch model)
        {
            try
            {
                int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                string roleId = User.FindFirst(ClaimTypes.Role)?.Value;

                // Admin-only for Delete & Archive
                if ((model.Type == 3 || model.Type == 4) && roleId != "1")
                {
                    return Unauthorized(new
                    {
                        isSuccess = false,
                        message = "You are not authorized to perform this action"
                    });
                }

                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("U77_Common")))
                using (SqlCommand cmd = new SqlCommand("[dbo].[U77_Pro_M13_batchoperation]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Type", model.Type);
                    cmd.Parameters.AddWithValue("@Id", (object?)model.Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@OrganizationId", (object?)model.OrganizationId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CollegeId", (object?)model.CollegeId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BranchId", (object?)model.BranchId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CourseTypeId", (object?)model.CourseTypeId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UniversityId", (object?)model.UniversityId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SectorId", (object?)model.SectorId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CourseId", (object?)model.CourseId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BatchCode", (object?)model.BatchCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BatchName", (object?)model.BatchName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BatchType", (object?)model.BatchType ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SoftSkillCapacity", (object?)model.SoftSkillCapacity ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CoreSkillCapacity", (object?)model.CoreSkillCapacity ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@usp_softskill", (object?)model.usp_softskill ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@usp_coreskill", (object?)model.usp_coreskill ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SDMSBatchId", (object?)model.SDMSBatchId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@StartDate", (object?)model.StartDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EndDate", (object?)model.EndDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BatchStatus", (object?)model.BatchStatus ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", (object?)model.Status ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Archive", (object?)model.Archive ?? DBNull.Value);
                    // stored proc uses @is_active, @Created_by, @Updated_by
                    cmd.Parameters.AddWithValue("@is_active", (object?)model.IsActive ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Created_by", userId);
                    cmd.Parameters.AddWithValue("@Updated_by", userId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    return Ok(new
                    {
                        isSuccess = Convert.ToBoolean(row["isSuccess"]),
                        message = row["message"]?.ToString(),
                        data = row["data"] == DBNull.Value
                            ? null
                            : System.Text.Json.JsonSerializer.Deserialize<object>(row["data"].ToString())
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