using System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References
{
    internal readonly struct MethodReferenceModel : IEquatable<MethodReferenceModel>
    {
        public string Name { get; }

        public ImmutableEquatableArray<TypeReferenceModel> Parameters { get; }

        public TypeReferenceModel Result { get; }

        public MethodReferenceModel(string methodName, ImmutableEquatableArray<TypeReferenceModel> parameters, TypeReferenceModel result)
        {
            Name = methodName ?? string.Empty;
            Parameters = parameters ?? ImmutableEquatableArray<TypeReferenceModel>.Empty;
            Result = result;
        }

        public bool Equals(MethodReferenceModel other)
        {
            return string.Equals(Name, other.Name, StringComparison.Ordinal)
                && Parameters.Equals(other.Parameters)
                && Result.Equals(other.Result);
        }

        public override bool Equals(object obj)
        {
            return obj is MethodReferenceModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                StringComparer.Ordinal.GetHashCode(Name),
                Parameters.GetHashCode(),
                Result.GetHashCode());
        }

        public static bool operator ==(MethodReferenceModel left, MethodReferenceModel right) => left.Equals(right);

        public static bool operator !=(MethodReferenceModel left, MethodReferenceModel right) => !left.Equals(right);
    }
}
