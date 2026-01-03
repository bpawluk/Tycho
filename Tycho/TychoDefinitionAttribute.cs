using System;

namespace Tycho
{
    /// <summary>
    /// Marks a class as a Tycho app definition.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TychoDefinitionAttribute : Attribute
    {
    }
}
