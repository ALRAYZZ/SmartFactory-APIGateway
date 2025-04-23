using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GatewayApi.Controllers
{
    [Route("login")]
    [ApiController]
    public class AuthController : ControllerBase
    {
		// Self reminder this DI is what allows us to access the appsettings.json file so we can create the JWT token out of the string on that file
		private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

        [HttpPost]
        public IActionResult Login([FromBody] LoginModel model)
        {
			// In-memory authentication for demonstration purposes'
            if (model.Username == "admin" && model.Password == "password")
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, model.Username),
                    new Claim(ClaimTypes.Role, "Admin")
				};

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
					audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(int.Parse(_configuration["Jwt:ExpireMinutes"])),
					signingCredentials: credentials);

                return Ok(new {Token = new JwtSecurityTokenHandler().WriteToken(token) });
			}

            return Unauthorized();
		}
	}

    public record LoginModel(string Username, string Password);
}
