using System;

namespace handshake.Data
{
  /// <summary>
  /// The <see cref="SimpleTimeSpan"/> class is a simple time span.
  /// </summary>
  public class SimpleTimeSpan
  {
    #region Constructors

    /// <summary>
    /// Creates a new <see cref="SimpleTimeSpan"/> from a system <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="timeSpan">The <see cref="TimeSpan"/>.</param>
    public SimpleTimeSpan(TimeSpan timeSpan)
    {
      this.Seconds = timeSpan.Seconds;
      this.Minutes = timeSpan.Minutes;
      this.TotalDays = (int)timeSpan.TotalDays;
    }

    /// <summary>
    /// Creates a new <see cref="SimpleTimeSpan"/> instance.
    /// </summary>
    public SimpleTimeSpan()
    {
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// The minutes part.
    /// </summary>
    public int Minutes { get; set; }

    /// <summary>
    /// The seconds part.
    /// </summary>
    public int Seconds { get; set; }

    /// <summary>
    /// The total days.
    /// </summary>
    public int TotalDays { get; set; }

    #endregion Properties
  }
}