using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;

namespace atmglobalapi.Controllers.TMS
{
    [ApiExplorerSettings(GroupName = "TMS")]
    [Tags("TMS")]
    [Route("api/tms")]
    [ApiController]
    public class T05CheckPreviousWorkingDayTMSController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public T05CheckPreviousWorkingDayTMSController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("T05CheckPreviousWorkingDayTMS_Detailed")]
        public IActionResult T05CheckPreviousWorkingDayTMS_Detailed([FromQuery] int userId)
        {
            try
            {
                string connStr = _configuration.GetConnectionString("Sqlserver_Connection_StringTask");

                var dayStatusList = new List<dynamic>();
                string finalMessage = "";
                bool isAllowed = true;

                using (SqlConnection con = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand("sp_CheckPreviousWorkingDayTMS_Detailed", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@Today", SqlDbType.Date).Value = DateTime.Today;

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // First resultset: day-wise status
                        while (reader.Read())
                        {
                            dayStatusList.Add(new
                            {
                                Date = Convert.ToDateTime(reader["Date"]).ToString("dd-MM-yyyy"),
                                Status = reader["Status"].ToString()
                            });
                        }

                        // Move to second resultset: overall message
                        if (reader.NextResult() && reader.Read())
                        {
                            isAllowed = Convert.ToBoolean(reader["IsAllowed"]);
                            finalMessage = reader["Message"].ToString();
                        }
                    }
                }

                return Ok(new
                {
                    isSuccess = isAllowed,
                    dayStatusList,
                    message = finalMessage
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
