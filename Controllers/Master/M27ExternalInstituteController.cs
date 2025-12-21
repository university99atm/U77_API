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
                // 🔐 JWT Claims
                int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                using SqlConnection con = new SqlConnection(
                    _configuration.GetConnectionString("U77_Common"));

                using SqlCommand cmd = new SqlCommand(
                    "U77_Pro_M27_ExternalInstitute_Operation", con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Type", model.Type);
                cmd.Parameters.AddWithValue("@Id", model.Id ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@InstituteName", model.InstituteName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@InstituteCode", model.InstituteCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@InstituteType", model.InstituteType ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@SectorId", model.SectorId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EducationBoardId", model.EducationBoardId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MediumOfInstructionId", model.MediumOfInstructionId ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@CountryId", model.CountryId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@StateId", model.StateId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DistrictId", model.DistrictId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CityId", model.CityId ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@AddressLine1", model.AddressLine1 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AddressLine2", model.AddressLine2 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Pincode", model.Pincode ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@Status", model.Status ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Archive", model.Archive ?? (object)DBNull.Value);

                // Audit fields
                cmd.Parameters.AddWithValue("@CreatedBy",
                    model.Type == 1 ? userId : (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@UpdatedBy",
                    model.Type == 2 || model.Type == 4 ? userId : (object)DBNull.Value);

                con.Open();

                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return Ok(new
                {
                    isSuccess = Convert.ToBoolean(dt.Rows[0]["isSuccess"]),
                    message = dt.Rows[0]["message"].ToString(),
                    data = dt.Rows[0]["data"] == DBNull.Value ? null : dt.Rows[0]["data"]
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }
    }
}
