using handshake.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace handshake.Controllers
{
  /// <summary>
  /// The <see cref="InviteController"/> provited functionality for user invites.
  /// </summary>
  [Route("[controller]")]
  [ApiController]
  public class InviteController : ControllerBase
  {
    #region Fields

    private readonly IConfiguration configuration;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Creates a new instance of the <see cref="InviteController"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public InviteController(IConfiguration configuration)
    {
      this.configuration = configuration;
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Shows an invite in the browser.
    /// </summary>
    /// <param name="id">The id for the invite.</param>
    /// <returns>The html document.</returns>
    [HttpGet]
    public ContentResult Get(Guid id)
    {
      string playStoreUrl = this.configuration["PlayStoreUrl"];

      var html = Resources.InvitePage
        .Replace("{id}", id.ToString())
        .Replace("{gp}", playStoreUrl);

      return base.Content(html, "text/html");
    }

    #endregion Methods
  }
}