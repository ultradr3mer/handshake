namespace handshake.Data
{
  /// <summary>
  /// The data needed to access a file.
  /// </summary>
  public class FileTokenData
  {
    #region Properties

    /// <summary>
    /// The file name.
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// The access token.
    /// </summary>
    public string Token { get; set; }

    #endregion Properties
  }
}