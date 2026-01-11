using atmglobalapi.Model.Master;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
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
    public class M27ExternalInstituteController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public M27ExternalInstituteController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("operation")]
        public IActionResult ExternalInstituteOperation([FromBody] M27ExternalInstitute model)
        {
            try
            {
                /* ================= JWT CLAIMS ================= */
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
                    new SqlConnection(_configuration.GetConnectionString("U77_Master")))
                using (SqlCommand cmd =
                    new SqlCommand("dbo.U77_Pro_M27_externalinstituteoperation", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Type", model.Type);
                    cmd.Parameters.AddWithValue("@Id", (object?)model.Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@InstituteName", (object?)model.InstituteName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@InstituteType", (object?)model.InstituteType ?? DBNull.Value);

                    // Foreign Keys
                    cmd.Parameters.AddWithValue("@EducationBoardId", (object?)model.EducationBoardId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MediumOfInstructionId", (object?)model.MediumOfInstructionId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CountryId", (object?)model.CountryId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@StateId", (object?)model.StateId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DistrictId", (object?)model.DistrictId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CityId", (object?)model.CityId ?? DBNull.Value);

                    // Address
                    cmd.Parameters.AddWithValue("@AddressLine1", (object?)model.AddressLine1 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AddressLine2", (object?)model.AddressLine2 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Pincode", (object?)model.Pincode ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@Status", (object?)model.Status ?? DBNull.Value);

                    // Pagination
                    cmd.Parameters.AddWithValue("@PageNumber", model.PageNumber ?? 1);
                    cmd.Parameters.AddWithValue("@PageSize", model.PageSize ?? 10);
                    cmd.Parameters.AddWithValue("@Search", (object?)model.Search ?? DBNull.Value);

                    // Audit
                    cmd.Parameters.AddWithValue("@System", model.System ?? false);
                    cmd.Parameters.AddWithValue("@IPAddress", ipAddress);
                    cmd.Parameters.AddWithValue("@operation_by", userId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                /* ================= RESPONSE ================= */
                if (dt.Rows.Count > 0)
                {
                    // Convert DataTable to a JSON-serializable structure (List<Dictionary<string, object>>)
                    var rows = dt.Rows.Cast<DataRow>()
                        .Select(r => dt.Columns.Cast<DataColumn>()
                            .ToDictionary(
                                c => c.ColumnName,
                                c => r[c] == DBNull.Value ? null : r[c]
                            )
                        )
                        .ToList();

                    return Ok(new
                    {
                        isSuccess = true,
                        data = rows
                    });
                }

                return Ok(new
                {
                    isSuccess = false,
                    message = "No data returned"
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
