using System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models.System
{
    public readonly struct MethodSignatureModel : IEquatable<MethodSignatureModel>
    {
        public string MethodName { get; }

        public ImmutableEquatableArray<TypeReferenceModel> Parameters { get; }

        public TypeReferenceModel Result { get; }

        public MethodSignatureModel(
            string methodName,
            ImmutableEquatableArray<TypeReferenceModel> parameters,
            TypeReferenceModel result)
        {
            MethodName = methodName ?? string.Empty;
            Parameters = parameters ?? ImmutableEquatableArray<TypeReferenceModel>.Empty;
            Result = result;
        }

        public bool Matches(MethodSignatureModel other)
        {
            return string.Equals(MethodName, other.MethodName, StringComparison.Ordinal) &&
                   Parameters.Count == other.Parameters.Count &&
                   Result.Matches(other.Result);
        }

        public bool Equals(MethodSignatureModel other)
        {
            return string.Equals(MethodName, other.MethodName, StringComparison.Ordinal) &&
                   Parameters.Equals(other.Parameters) &&
                   Result.Equals(other.Result);
        }

        public override bool Equals(object obj)
        {
            return obj is MethodSignatureModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                StringComparer.Ordinal.GetHashCode(MethodName),
                Parameters.GetHashCode(),
                Result.GetHashCode());
        }

        public static bool operator ==(MethodSignatureModel left, MethodSignatureModel right) => left.Equals(right);

        public static bool operator !=(MethodSignatureModel left, MethodSignatureModel right) => !left.Equals(right);
    }
}
