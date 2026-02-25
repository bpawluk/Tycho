using System;
using Tycho.Utils.SourceGenerator.References;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models.System
{
    public readonly struct TypeArgument : IEquatable<TypeArgument>
    {
        public string Name { get; }

        public TypeModel Value { get; }

        public bool IsRequestType =>
            Name == IAppContractReference.RequestTypeParameterName ||
            Name == IModuleContractReference.RequestTypeParameterName;

        public bool IsResponseType =>
            Name == IAppContractReference.ResponseTypeParameterName ||
            Name == IModuleContractReference.ResponseTypeParameterName;

        public bool IsEventType =>
            Name == IAppEventsReference.EventTypeParameterName || 
            Name == IModuleEventsReference.EventTypeParameterName;

        public bool IsModuleType =>
            Name == IAppStructureReference.ModuleTypeParameterName ||
            Name == IModuleStructureReference.ModuleTypeParameterName;

        public TypeArgument(string name, TypeModel value)
        {
            Name = name;
            Value = value;
        }

        public bool Equals(TypeArgument other)
        {
            return string.Equals(Name, other.Name, StringComparison.Ordinal)
                && Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            return obj is TypeArgument other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                StringComparer.Ordinal.GetHashCode(Name ?? string.Empty),
                Value.GetHashCode());
        }

        public static bool operator ==(TypeArgument left, TypeArgument right) => left.Equals(right);

        public static bool operator !=(TypeArgument left, TypeArgument right) => !left.Equals(right);
    }
}
