using atmglobalapi.Model.TMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;

namespace atmglobalapi.Controllers.TMS
{
    [ApiExplorerSettings(GroupName = "TMS")]
    [Tags("TMS")]
    [Route("api/tms")]
    [ApiController]
    public class T01GetUserTMSStatusController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public T01GetUserTMSStatusController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("T01GetUserTMSStatus")]
        public IActionResult T01GetUserTMSStatus([FromQuery] int userId, [FromQuery] DateTime date)
        {
            try
            {
                TMSStatusModel? result = null;

                using (SqlConnection con =
                    new SqlConnection(_configuration.GetConnectionString("COMMERPDb1")))
                using (SqlCommand cmd =
                    new SqlCommand("GetUserTMSStatus", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@Date", SqlDbType.Date).Value = date;

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = new TMSStatusModel
                            {
                                UserId = Convert.ToInt32(reader["UserId"]),
                                Date = Convert.ToDateTime(reader["Date"]),
                                TMSStatus = reader["TMSStatus"]?.ToString()
                            };
                        }
                    }
                }

                if (result == null)
                {
                    return NotFound(new
                    {
                        isSuccess = false,
                        message = "No TMS status found"
                    });
                }

                return Ok(new
                {
                    isSuccess = true,
                    data = result
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
