using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace atmglobalapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TestController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("dbtest")]
        public IActionResult DbTest()
        {
            string connectionString = _configuration.GetConnectionString("Connection");

            DataTable dt = new DataTable();
            string query = @"SELECT TOP 10 id, uid, pwd, useremail, rollid, status, archive 
                             FROM Atm_T_UserDetails88";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                // ✅ Convert DataTable to List of Dictionary
                var rows = new List<Dictionary<string, object>>();
                foreach (DataRow row in dt.Rows)
                {
                    var dict = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        dict[col.ColumnName] = row[col];
                    }
                    rows.Add(dict);
                }

                return Ok(rows);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Database error", error = ex.Message });
            }
        }
    }
}
