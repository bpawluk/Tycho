using System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Model.Partial
{
    public readonly struct TypeParameter : IEquatable<TypeParameter>
    {
        public string ParameterName { get; }

        public TypeModel ParameterValue { get; }

        public TypeParameter(string parameterName, TypeModel parameterValue)
        {
            ParameterName = parameterName;
            ParameterValue = parameterValue;
        }

        public bool Equals(TypeParameter other)
        {
            return string.Equals(ParameterName, other.ParameterName, StringComparison.Ordinal)
                && ParameterValue.Equals(other.ParameterValue);
        }

        public override bool Equals(object obj)
        {
            return obj is TypeParameter other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                StringComparer.Ordinal.GetHashCode(ParameterName ?? string.Empty),
                ParameterValue.GetHashCode());
        }

        public static bool operator ==(TypeParameter left, TypeParameter right) => left.Equals(right);

        public static bool operator !=(TypeParameter left, TypeParameter right) => !left.Equals(right);
    }
}
