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
    public class GetStudentRollNoController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public GetStudentRollNoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Generate next Student Roll No based on BatchCode
        /// Format: FY/BatchCode/001
        /// </summary>
        [HttpGet("generate/{batchCode}")]
        public IActionResult GenerateStudentRollNo(string batchCode)
        {
            try
            {
                DataTable dt = new DataTable();

                using (SqlConnection con =
                    new SqlConnection(_configuration.GetConnectionString("U77_Common")))
                using (SqlCommand cmd =
                    new SqlCommand("[dbo].[U77_Pro_GetNextStudentRollNo]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BatchCode", batchCode);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                if (dt.Rows.Count == 0)
                {
                    return Ok(new
                    {
                        isSuccess = false,
                        message = "Student Roll No not generated",
                        data = (object?)null
                    });
                }

                return Ok(new
                {
                    isSuccess = Convert.ToBoolean(dt.Rows[0]["isSuccess"]),
                    message = dt.Rows[0]["message"]?.ToString(),
                    data = new
                    {
                        StudentRollNo = dt.Rows[0]["StudentRollNo"]?.ToString()
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
