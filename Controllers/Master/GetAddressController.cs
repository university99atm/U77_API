using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using atmglobalapi.Model.Master;

namespace atmglobalapi.Controllers.Master
{
    [ApiExplorerSettings(GroupName = "Master")]
    [Tags("Master")]
    [Route("api/master/[controller]")]
    [ApiController]
    [Authorize]
    public class GetAddressController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public GetAddressController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("by-pincode/{pincode}")]
        public IActionResult GetLocationByPincode(string pincode)
        {
            try
            {
                DataTable dt = new DataTable();

                using (SqlConnection con =
                    new SqlConnection(_configuration.GetConnectionString("U77_Common")))
                using (SqlCommand cmd =
                    new SqlCommand("[dbo].[U77_Pro_GetLocationByPincode]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Pincode", pincode);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                if (dt.Rows.Count == 0)
                {
                    return Ok(new
                    {
                        isSuccess = false,
                        message = "No location found for this pincode",
                        data = (object?)null   // ✅ FIX
                    });
                }

                var data = dt.AsEnumerable().Select(row => new
                {
                    Country = new
                    {
                        Id = row.Field<int>("CountryId"),
                        Name = (string?)row.Field<string>("CountryName")
                    },
                    State = new
                    {
                        Id = row.Field<int>("StateId"),
                        Name = (string?)row.Field<string>("StateName"),
                        Code = (string?)row.Field<string>("StateCode")
                    },
                    District = new
                    {
                        Id = row.Field<int>("DistrictId"),
                        Name = (string?)row.Field<string>("DistrictName")
                    },
                    City = new M05City()
                    {
                        Id = row.Field<int?>("CityId"),
                        CityName = (string?)row.Field<string>("CityName"),
                        CityCode = (string?)row.Field<string>("CityCode")
                    },
                    Area = new
                    {
                        Id = row.Field<int>("AreaId"),
                        Name = (string?)row.Field<string>("AreaName"),
                        Pincode = (string?)row.Field<string>("Pincode")
                    }
                }).ToList();

                return Ok(new
                {
                    isSuccess = true,
                    message = "Location fetched successfully",
                    data
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
