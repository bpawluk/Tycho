using System;

namespace Tycho.Apps
{
    /// <summary>
    /// Marks a class as a Tycho app definition.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AppDefinitionAttribute : Attribute
    {
    }
}
