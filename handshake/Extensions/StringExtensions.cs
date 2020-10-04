using System.Globalization;

namespace handshake.Extensions
{
  internal static class StringExtensions
  {
    internal static bool ContainsIgnoreCase(this string source, string value)
    {
      return CultureInfo.InvariantCulture.CompareInfo.IndexOf(source, value, CompareOptions.IgnoreCase) >= 0;
    }
  }
}
