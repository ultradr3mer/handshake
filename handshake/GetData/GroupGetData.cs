﻿using handshake.Data;
using handshake.Entities;
using handshake.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace handshake.GetData
{
  /// <summary>
  /// The <see cref="GroupDetailGetData"/> contains information about the group.
  /// </summary>
  public class GroupGetData
  {
    #region Constructors

    /// <summary>
    /// Creates a new <see cref="GroupDetailGetData"/> based on the <see cref="GroupEntity"/>.
    /// </summary>
    /// <param name="entity">The <see cref="GroupEntity"/> to copy properties from.</param>
    public GroupGetData(GroupEntity entity)
    {
      this.CopyPropertiesFrom(entity);
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// The description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The associated icon.
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// The id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The name of the group.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The name of the owner.
    /// </summary>
    public string OwnerName { get; set; }

    #endregion Properties
  }
}