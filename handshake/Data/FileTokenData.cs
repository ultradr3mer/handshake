using handshake.Entities;
using System;

namespace handshake.Data
{
  /// <summary>
  /// The data needed to access a file.
  /// </summary>
  public class FileTokenData
  {
    #region Constructors

    /// <summary>
    /// Create a new instance of the <see cref="FileTokenData"/> class.
    /// </summary>
    public FileTokenData() { }

    /// <summary>
    /// Create a new instance of the <see cref="FileTokenData"/> class from the given entity.
    /// </summary>
    /// <param name="entity">The base entity.</param>
    public FileTokenData(FileAccessTokenEntity entity)
    {
      this.Id = entity.Id;
      this.Filename = entity.Filename;
      this.Token = entity.Token.ToString("X");
    }

    #endregion Constructors

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

    #endregion Properties

    #region Methods

    /// <summary>
    /// Generates the url.
    /// </summary>
    /// <returns>The local url.</returns>
    internal string GetUrl()
    {
      return Token + "/" + Filename;
    }

    #endregion Methods
  }
}