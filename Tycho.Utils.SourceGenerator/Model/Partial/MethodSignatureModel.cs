using System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Model.Partial
{
    public readonly struct MethodSignatureModel : IEquatable<MethodSignatureModel>
    {
        public string MethodName { get; }

        public ImmutableEquatableArray<TypeModel> Parameters { get; }

        public TypeModel Result { get; }

        public MethodSignatureModel(
            string methodName,
            ImmutableEquatableArray<TypeModel> parameters,
            TypeModel result)
        {
            MethodName = methodName;
            Parameters = parameters;
            Result = result;
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
                StringComparer.Ordinal.GetHashCode(MethodName ?? string.Empty),
                Parameters.GetHashCode(),
                Result.GetHashCode());
        }

        public static bool operator ==(MethodSignatureModel left, MethodSignatureModel right) => left.Equals(right);

        public static bool operator !=(MethodSignatureModel left, MethodSignatureModel right) => !left.Equals(right);
    }
}
