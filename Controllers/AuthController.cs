using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using atmglobalapi.Models;
using CommonClass;
using atmglobalapi.Services; // ✅ for JwtService

namespace atmglobalapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IJwtService _jwtService;

        public AuthController(IConfiguration configuration, IJwtService jwtService)
        {
            _configuration = configuration;
            _jwtService = jwtService;
        }

        [HttpPost("studentapi/login")]
        public IActionResult Login([FromBody] UserLoginRequest model)
        {
            if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
                return BadRequest("Username or Password cannot be empty.");

            try
            {
                CommonFunction cmf = new CommonFunction();
                string encryptedPassword = cmf.Encrypt(model.Password);
                string connectionString = _configuration.GetConnectionString("Connection");

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("[user_login]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@name", model.Username);
                    cmd.Parameters.AddWithValue("@password", encryptedPassword);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    var decryptedPwd = cmf.Decrypt(row["pwd"].ToString());
                    var roleId = row["rollid"].ToString();
                    var userId = row["uid"].ToString();

                    // ✅ Generate JWT token
                    var token = _jwtService.GenerateToken(userId, decryptedPwd, roleId);

                    return Ok(new
                    {
                        Success = true,
                        Message = "Login successful",
                        Token = token,
                        RoleId = roleId,
                        UserId = userId,
                        Email = decryptedPwd
                    });
                }
                else
                {
                    return Ok(new { Success = false, Message = "Invalid username or password" });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    Success = false,
                    Message = "Error during login process",
                    ExceptionMessage = ex.Message,
                    ExceptionType = ex.GetType().ToString()
                });
            }
        }

        // ✅ Decode Token Endpoint
        [HttpPost("decode")]
        public IActionResult DecodeToken([FromBody] string token)
        {
            try
            {
                var claims = _jwtService.DecodeToken(token);
                return Ok(new { Success = true, Claims = claims });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = "Invalid token", Error = ex.Message });
            }
        }
    }
}
