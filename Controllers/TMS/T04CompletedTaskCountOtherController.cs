using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using atmglobalapi.Model.Tasks;

namespace atmglobalapi.Controllers.Tasks
{
    [ApiExplorerSettings(GroupName = "TMS")]
    [Tags("TMS")]
    [Route("api/tms")]
    [ApiController]
    public class T04CompletedTaskCountOtherController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public T04CompletedTaskCountOtherController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("T04CompletedTaskCountOther")]
        public IActionResult T04CompletedTaskCountOther([FromQuery] int responsiblePersonId)
        {
            try
            {
                int taskOwnerId = 0;
                int tOption = 102;
                int roleId = 0;

                string connStr =
                    _configuration.GetConnectionString("Sqlserver_Connection_StringTask");

                /* ================= STEP 1 : GET ROLE ================= */
                using (SqlConnection con = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand("sp_GetUserRoleAndLimit", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ResponsiblePersonId", SqlDbType.Int)
                                   .Value = responsiblePersonId;

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            roleId = Convert.ToInt32(reader["Role_id"]);
                        }
                    }
                }

                /* ================= STEP 2 : GET TASK LIST ================= */
                List<TaskViewModel> tasks = new List<TaskViewModel>();

                using (SqlConnection con = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand("sp_TMS_Search_TaskMasterDetails_Other", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@tOption", SqlDbType.Int).Value = tOption;
                    cmd.Parameters.Add("@responsiblePersonId", SqlDbType.Int).Value = responsiblePersonId;
                    cmd.Parameters.Add("@taskOwnerId", SqlDbType.Int).Value = taskOwnerId;
                    cmd.Parameters.Add("@taskTitle", SqlDbType.VarChar).Value = DBNull.Value;

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tasks.Add(new TaskViewModel
                            {
                                taskStageName = reader["taskStageName"]?.ToString()
                            });
                        }
                    }
                }

                int completedTaskCount =
                    tasks.Count(x => x.taskStageName == "Completed");

                /* ================= RESPONSE ================= */
                return Ok(new
                {
                    isSuccess = true,
                    responsiblePersonId,
                    roleId,
                    completedTaskCount,
                    message = completedTaskCount > 0
                        ? $"You have {completedTaskCount} completed task(s)."
                        : "No completed tasks found."
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
