using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace atmglobalapi.Controllers.Master
{
    [ApiExplorerSettings(GroupName = "Master")]
    [Tags("Master")]
    [Route("api/master/[controller]")]
    [ApiController]
    [Authorize]
    public class GetSRNController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public GetSRNController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("generate")]
        public IActionResult GenerateSRN()
        {
            try
            {
                DataTable dt = new DataTable();

                using (SqlConnection con =
                    new SqlConnection(_configuration.GetConnectionString("U77_Common")))
                using (SqlCommand cmd =
                    new SqlCommand("[dbo].[U77_Pro_GetNextSRN_Simple]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                if (dt.Rows.Count == 0)
                {
                    return Ok(new
                    {
                        isSuccess = false,
                        message = "SRN not generated",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    isSuccess = Convert.ToBoolean(dt.Rows[0]["isSuccess"]),
                    message = dt.Rows[0]["message"]?.ToString(),
                    data = new
                    {
                        SRN = dt.Rows[0]["SRN"]?.ToString()
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
    }
}
