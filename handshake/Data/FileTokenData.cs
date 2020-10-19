using System;

namespace handshake.Data
{
  /// <summary>
  /// The data needed to access a file.
  /// </summary>
  public class FileTokenData
  {
    #region Properties

    /// <summary>
    /// The filename.
    /// </summary>
    public string Filename { get; set; }

    /// <summary>
    /// The id of the token.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The access token.
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Generates the url.
    /// </summary>
    /// <returns>The local url.</returns>
    internal string GetUrl()
    {
      return Token + "/" + Filename;
    }

    #endregion Properties
  }
}