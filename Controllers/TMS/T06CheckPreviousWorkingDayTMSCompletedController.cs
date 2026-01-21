using atmglobalapi.Model.TMS;
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
    public class T06CheckPreviousWorkingDayTMSCompletedController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public T06CheckPreviousWorkingDayTMSCompletedController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("T06CheckPreviousWorkingDayTMSCompleted")]
        public IActionResult T06CheckPreviousWorkingDayTMSCompleted([FromQuery] int userId)
        {
            try
            {
                var dayStatusList = new List<TMSDayStatusModel>();
                int isAllowed = 0;
                string message = "";
                int totalCompletedPoints = 0;

                using var con = new SqlConnection(_configuration.GetConnectionString("Sqlserver_Connection_StringTask"));
                using var cmd = new SqlCommand("dbo.sp_CheckPreviousWorkingDayTMS_DetailedCompleted", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);

                con.Open();
                using var reader = cmd.ExecuteReader();

                // 🔹 Day-wise Status (First Result Set)
                while (reader.Read())
                {
                    DateTime dayDate = DateTime.Today;
                    string status = "";

                    // Safe access by ordinal (index) or column name
                    int dateIndex = reader.GetOrdinal("Date");
                    int statusIndex = reader.GetOrdinal("Status");

                    if (!reader.IsDBNull(dateIndex))
                        dayDate = reader.GetDateTime(dateIndex);

                    if (!reader.IsDBNull(statusIndex))
                        status = reader.GetString(statusIndex);

                    dayStatusList.Add(new TMSDayStatusModel
                    {
                        Date = dayDate,
                        Status = status
                    });
                }

                // 🔹 Final decision (Second Result Set)
                if (reader.NextResult() && reader.Read())
                {
                    if (!reader.IsDBNull(reader.GetOrdinal("IsAllowed")))
                        isAllowed = reader.GetInt32(reader.GetOrdinal("IsAllowed"));

                    if (!reader.IsDBNull(reader.GetOrdinal("Message")))
                        message = reader.GetString(reader.GetOrdinal("Message"));

                    if (!reader.IsDBNull(reader.GetOrdinal("TotalCompletedPoints")))
                        totalCompletedPoints = reader.GetInt32(reader.GetOrdinal("TotalCompletedPoints"));
                }

                return Ok(new
                {
                    isSuccess = true,
                    dayStatusList,
                    isAllowed,
                    totalCompletedPoints,
                    message
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
