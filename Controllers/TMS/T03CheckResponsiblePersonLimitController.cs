using atmglobalapi.Model.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace atmglobalapi.Controllers.Tasks
{
    [ApiExplorerSettings(GroupName = "TMS")]
    [Tags("TMS")]
    [Route("api/tms")]
    [ApiController]
    public class T03CheckResponsiblePersonLimitController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public T03CheckResponsiblePersonLimitController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("T03CheckResponsiblePersonLimit")]
        public IActionResult T03CheckResponsiblePersonLimit([FromQuery] int responsiblePersonId)
        {
            try
            {
                int roleId = 0;
                int maxPendingLimit = 0;
                int totalLosePoints = 0;
                int pendingTaskCount = 0;

                string connStr =
                    _configuration.GetConnectionString("Sqlserver_Connection_StringTask");

                /* ================= STEP 1 : ROLE & LIMIT ================= */
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
                            maxPendingLimit = Convert.ToInt32(reader["MaxPendingLimit"]);
                        }
                    }
                }

                /* ================= STEP 2 : GET TASK LIST ================= */
                List<TaskViewModel> tasks = new List<TaskViewModel>();

                using (SqlConnection con = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand("sp_TMS_Search_TaskMasterDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@responsiblePersonId", SqlDbType.Int)
                                   .Value = responsiblePersonId;
                    cmd.Parameters.Add("@taskTitle", SqlDbType.VarChar)
                                   .Value = DBNull.Value;

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tasks.Add(new TaskViewModel
                            {
                                taskStageName = reader["taskStageName"]?.ToString(),
                                losePoint = Convert.ToInt32(reader["losePoint"]),
                                actualPoint = Convert.ToInt32(reader["actualPoint"])
                            });
                        }
                    }
                }

                /* ================= STEP 3 : CALCULATION ================= */
                var pendingTasks = tasks
                    .Where(x => x.taskStageName == "Inprogress"
                             || x.taskStageName == "New Task")
                    .ToList();

                totalLosePoints = pendingTasks.Sum(x => x.losePoint);
                pendingTaskCount = pendingTasks.Count;
                int totalActualPoints = pendingTasks.Sum(x => x.actualPoint);

                /* ================= STEP 4 : BUSINESS RULES ================= */
                if (roleId != 5 && totalLosePoints >= maxPendingLimit)
                {
                    return Ok(new
                    {
                        isSuccess = false,
                        status = "error",
                        totalLosePoints,
                        maxPendingLimit,
                        pendingTaskCount,
                        message = "Responsible person has exceeded the maximum pending limit."
                    });
                }

                if (roleId == 2 && totalLosePoints > 0)
                {
                    return Ok(new
                    {
                        isSuccess = false,
                        status = "error",
                        totalLosePoints,
                        maxPendingLimit,
                        pendingTaskCount,
                        message = "You have pending tasks to approve. Please clear them before you can log in."
                    });
                }

                /* ================= SUCCESS ================= */
                return Ok(new
                {
                    isSuccess = true,
                    status = "success",
                    totalActualPoint = totalActualPoints,
                    maxPendingLimit,
                    pendingTaskCount,
                    message = "Check passed successfully."
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
