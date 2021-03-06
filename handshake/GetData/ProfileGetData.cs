﻿using handshake.Data;
using System;
using System.Collections.Generic;

namespace handshake.GetData
{
  /// <summary>
  /// A user.
  /// </summary>
  public class ProfileGetData
  {
    #region Properties

    /// <summary>
    /// The url of the Avatar.
    /// </summary>
    public string Avatar { get; set; }

    /// <summary>
    /// The description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The users nickname.
    /// </summary>
    public string Nickname { get; set; }

    /// <summary>
    /// The associated groups.
    /// </summary>
    public List<GroupGetData> Groups { get; set; }

    #endregion Properties
  }
}