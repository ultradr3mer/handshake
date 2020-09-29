using System.Linq;
using System.Threading.Tasks;
using handshake.Contexts;
using handshake.Entities;
using handshake.ExtensionMethods;
using handshake.Interfaces;
using handshake.PostDaten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace handshake.Controllers
{
  /// <summary>
  /// The user controller provides functionality to manage posts.
  /// </summary>
  [Authorize]
  [ApiController]
  [Route("[controller]")]
  public class UserController : ControllerBase
  {
    private readonly IAuthService userService;

    /// <summary>
    /// Creates a new instance of the UserController class.
    /// </summary>
    /// <param name="userService">The user / login service.</param>
    public UserController(IAuthService userService)
    {
      this.userService = userService;
    }

    /// <summary>
    /// Gets all users nearby.
    /// </summary>
    /// <returns>Retruncode ok, when the retrival was a success.</returns>
    [HttpGet]
    [Route("getcloseusers")]
    public IActionResult GetCloseUsers(decimal Longitude, decimal Latitude)
    {
      using var connection = this.userService.GetConnection();
      var context = new DatabaseContext(connection);

      return Ok(context.User.ToList());
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="daten">The user to create.</param>
    /// <returns>Retruncode ok, whenn success.</returns>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] InsertUserDaten daten)
    {
      using var connection = this.userService.GetConnection();
      var context = new DatabaseContext(connection);
      var newUser = new User();
      newUser.CopyPropertiesFrom(daten);
      await context.User.AddAsync(newUser);
      await context.SaveChangesAsync();
      return Ok();
    }
  }
}
