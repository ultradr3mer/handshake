using System.Text.RegularExpressions;

namespace handshake.Const
{
  /// <summary>
  /// The <see cref="RegularExpressions"/> class contains regular expressions.
  /// </summary>
  internal static class RegularExpressions
  {
    #region Fields

    internal static readonly Regex AlphanumericRegex = new Regex("^[a-zA-Z0-9]*$", RegexOptions.Compiled);
    internal static readonly Regex Base10DigitRegex = new Regex("[0-9]", RegexOptions.Compiled);
    internal static readonly Regex LatinLowercaseRegex = new Regex("[a-z]", RegexOptions.Compiled);
    internal static readonly Regex LatinUppercaseRegex = new Regex("[A-Z]", RegexOptions.Compiled);
    internal static readonly Regex NonAlphanumericRegex = new Regex("[^a-zA-Z0-9]", RegexOptions.Compiled);
    internal static readonly Regex HashtagGroupRegex = new Regex("#(?<name>[a-zA-Z0-9]*)", RegexOptions.Compiled);

    #endregion Fields
  }
}