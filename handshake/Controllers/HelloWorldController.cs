using System.Text;
using handshake.Contexts;
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

    [HttpGet]
    public IActionResult Get()
    {
      using (var connection = this.userService.GetConnection())
      {
        var context = new DatabaseContext(connection);

        var sb = new StringBuilder();
        foreach (var item in context.User)
        {
          sb.Append(item.Nickname);
        }

        return Ok(sb);
      }
    }
  }
}
