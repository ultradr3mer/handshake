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
      var context = new PersonInfoContext(this.userService.GetConnection());

      var sb = new StringBuilder();
      foreach (var item in context.People)
      {
        sb.Append(item.Name);
        sb.Append(' ');
        sb.AppendLine(item.Name2);
      }

      return Ok(sb);
    }
  }
}
