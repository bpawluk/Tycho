using System;
using Tycho.Utils;

namespace Tycho
{
    /// <summary>
    /// Marks a class as a Tycho definition.
    /// </summary>
    [ReferencedBySourceGenerator]
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TychoDefinitionAttribute : Attribute
    {
    }
}
