using System;

namespace Tycho.Modules
{
    /// <summary>
    /// Marks a class as a Tycho app definition.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleDefinitionAttribute : Attribute
    {
    }
}
