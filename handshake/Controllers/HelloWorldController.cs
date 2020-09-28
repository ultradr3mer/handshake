using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using handshake.Data;
using handshake.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace handshake.Controllers
{
  [Authorize]
  [ApiController]
  [Route("[controller]")]
  public class HelloWorldController : ControllerBase
  {
    private readonly IUserService userService;

    public HelloWorldController(IUserService userService)
    {
      this.userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateModel model)
    {
      var connection = await userService.Authenticate(model.Username, model.Password);

      return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      using (var connection = this.userService.GetConnection())
      {
        string sql = "SELECT NAME, NAME2, AGE FROM PEOPLE";
        using (SqlCommand command = new SqlCommand(sql))
        {
          using (SqlDataReader reader = command.ExecuteReader())
          {
            StringBuilder sb = new StringBuilder();

            while (reader.Read())
            {
              sb.Append(reader.GetString(0));
              sb.Append(' ');
              sb.AppendLine(reader.GetString(1));
            }

            return Ok(sb.ToString());
          }
        }
      }
    }
  }
}
