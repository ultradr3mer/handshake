using System;

namespace handshake.Classes
{

  /// <summary>
  /// 
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class MatchParentAttribute : Attribute
  {
    public readonly string ParentPropertyName;
    public MatchParentAttribute(string parentPropertyName)
    {
      ParentPropertyName = parentPropertyName;
    }
  }
}
