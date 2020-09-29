using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using handshake.Contexts;
using handshake.Entities;
using handshake.ExtensionMethods;
using handshake.PostDaten;
using handshake.Services;
using handshake.SetDaten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace handshake.Controllers
{
  [Authorize]
  [ApiController]
  [Route("[controller]")]
  public class UserController : ControllerBase
  {
    private readonly IUserService userService;

    public UserController(IUserService userService)
    {
      this.userService = userService;
    }

    public async Task<IActionResult> Post([FromBody] InsertUserDaten daten)
    {
      using (var connection = this.userService.GetConnection())
      {
        var context = new DatabaseContext(connection);
        var newUser = new User();
        newUser.CopyPropertiesFrom(daten);
        await context.User.AddAsync(newUser);
        await context.SaveChangesAsync();
        return Ok();
      }
    }

    [HttpGet]
    [Route("getcloseusers")]
    public IActionResult GetCloseUsers(decimal Longitude, decimal Latitude)
    {
      using (var connection = this.userService.GetConnection())
      {
        var context = new DatabaseContext(connection);

        return Ok(context.User.ToList());
      }
    }
  }
}
