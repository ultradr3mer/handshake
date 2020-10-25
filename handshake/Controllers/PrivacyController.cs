using handshake.Properties;
using Microsoft.AspNetCore.Mvc;
using System;

namespace handshake.Controllers
{
  /// <summary>
  /// The <see cref="PrivacyController"/> delivers the privay policy.
  /// </summary>
  [Route("[controller]")]
  [ApiController]
  public class PrivacyController : ControllerBase
  {
    #region Constructors

    /// <summary>
    /// Creates a new instance of the <see cref="PrivacyController"/> class.
    /// </summary>
    public PrivacyController() { }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Shows the privacy policy in the browser.
    /// </summary>
    /// <returns>The html document.</returns>
    [HttpGet]
    public ContentResult Get()
    {
      var html = Resources.PrivacyPolicy;
      return base.Content(html, "text/html");
    }

    #endregion Methods
  }
}