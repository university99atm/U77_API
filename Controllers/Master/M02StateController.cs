using atmglobalapi.Model.Master;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;

[ApiExplorerSettings(GroupName = "Master")]
[Tags("Master")]
[Route("api/master/[controller]")]
[ApiController]
[Authorize]
public class M02StateController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public M02StateController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("operation")]
    public IActionResult StateOperation([FromBody] M02State model)
    {
        try
        {
            int userId = Convert.ToInt32(
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            DataTable dt = new DataTable();

            using SqlConnection con =
                new SqlConnection(_configuration.GetConnectionString("U77_Master"));

            using SqlCommand cmd =
                new SqlCommand("dbo.U77_Pro_M02_stateoperation", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Type", model.Type);
            cmd.Parameters.AddWithValue("@Id", (object?)model.Id ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CountryId", (object?)model.CountryId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@StateName", (object?)model.StateName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@StateCode", (object?)model.StateCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", (object?)model.Status ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@PageNumber", model.PageNumber ?? 1);
            cmd.Parameters.AddWithValue("@PageSize", model.PageSize ?? 10);
            cmd.Parameters.AddWithValue("@Search", (object?)model.Search ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@System", model.System ?? false);
            cmd.Parameters.AddWithValue("@IPAddress",
                HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString());

            cmd.Parameters.AddWithValue("@operation_by", userId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            var data = dt.Rows.Cast<DataRow>()
                .Select(r => dt.Columns.Cast<DataColumn>()
                    .ToDictionary(c => c.ColumnName,
                                  c => r[c] == DBNull.Value ? null : r[c]))
                .ToList();

            return Ok(new { isSuccess = true, data });
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
