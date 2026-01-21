using atmglobalapi.Model.Attendance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;

namespace atmglobalapi.Controllers.Attendance
{
    [ApiExplorerSettings(GroupName = "TMS")]
    [Tags("TMS")]
    [Route("api/tms")]
    [ApiController]
    public class T02GetLatestAttendanceStatusController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public T02GetLatestAttendanceStatusController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("T02GetLatestAttendanceStatus")]
        public IActionResult T02GetLatestAttendanceStatus([FromQuery] int userId)
        {
            try
            {
                AttendanceStatusModel? result = null;

                using (SqlConnection con =
                    new SqlConnection(_configuration.GetConnectionString("COMMERPDb1")))
                using (SqlCommand cmd =
                    new SqlCommand("GetLatestAttendanceStatus", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = new AttendanceStatusModel
                            {
                                RowId = Convert.ToInt32(reader["RowId"]),
                                UserId = Convert.ToInt32(reader["UserId"]),
                                Date = Convert.ToDateTime(reader["Date"]),
                                Day = reader["Day"]?.ToString(),
                                Shift = reader["Shift"]?.ToString(),
                                In_Time = reader["In_Time"] as TimeSpan?,
                                Out_Time = reader["Out_Time"] as TimeSpan?,
                                In_Location = reader["In_Location"]?.ToString(),
                                Out_Location = reader["Out_Location"]?.ToString(),
                                Working_Hour = reader["Working_Hour"]?.ToString(),
                                OT_Hour = reader["OT_Hour"]?.ToString(),
                                Less_Worked_Hour = reader["Less_Worked_Hour"]?.ToString(),
                                OpsStatus = reader["OpsStatus"]?.ToString(),
                                Status = reader["Status"]?.ToString(),
                                Remark = reader["Remark"]?.ToString(),
                                Hr_Remarks = reader["Hr_Remarks"]?.ToString(),
                                In_Pic = reader["In_Pic"]?.ToString(),
                                Out_Pic = reader["Out_Pic"]?.ToString(),
                                In_Ip = reader["In_Ip"]?.ToString(),
                                Out_Ip = reader["Out_Ip"]?.ToString(),
                                IsAgree = reader["isagree"] as bool?,
                                RequestInTime = reader["request_in_time"] as TimeSpan?,
                                RequestOutTime = reader["request_out_time"] as TimeSpan?,
                                Incurrentlocationname = reader["Incurrentlocationname"]?.ToString(),
                                Outcurrentlocationname = reader["Outcurrentlocationname"]?.ToString(),
                                Inlocationname = reader["Inlocationname"]?.ToString(),
                                Outlocationname = reader["Outlocationname"]?.ToString(),
                                AttendanceStatus = reader["AttendanceStatus"]?.ToString()
                            };
                        }
                    }
                }

                if (result == null)
                {
                    return NotFound(new
                    {
                        isSuccess = false,
                        message = "No attendance record found"
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
