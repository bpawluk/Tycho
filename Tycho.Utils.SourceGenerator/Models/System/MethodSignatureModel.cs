using System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models.System
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
                   ParametersAreEquivalent(Parameters, other.Parameters) &&
                   TypesAreEquivalent(Result, other.Result);
        }

        public override bool Equals(object obj)
        {
            return obj is MethodSignatureModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                StringComparer.Ordinal.GetHashCode(MethodName ?? string.Empty),
                GetTypeArrayHashCode(Parameters),
                GetTypeHashCode(Result));
        }

        public static bool operator ==(MethodSignatureModel left, MethodSignatureModel right) => left.Equals(right);

        public static bool operator !=(MethodSignatureModel left, MethodSignatureModel right) => !left.Equals(right);

        private static bool ParametersAreEquivalent(ImmutableEquatableArray<TypeModel> left, ImmutableEquatableArray<TypeModel> right)
        {
            ImmutableEquatableArray<TypeModel> leftValues = left ?? ImmutableEquatableArray<TypeModel>.Empty;
            ImmutableEquatableArray<TypeModel> rightValues = right ?? ImmutableEquatableArray<TypeModel>.Empty;
            if (leftValues.Count != rightValues.Count)
            {
                return false;
            }

            for (int i = 0; i < leftValues.Count; i++)
            {
                if (!TypesAreEquivalent(leftValues[i], rightValues[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool TypesAreEquivalent(TypeModel left, TypeModel right)
        {
            if (!string.Equals(left.Namespace, right.Namespace, StringComparison.Ordinal)
                || !string.Equals(left.Name, right.Name, StringComparison.Ordinal))
            {
                return false;
            }

            ImmutableEquatableArray<ContainingTypeModel> leftContaining = left.ContainingTypes ?? ImmutableEquatableArray<ContainingTypeModel>.Empty;
            ImmutableEquatableArray<ContainingTypeModel> rightContaining = right.ContainingTypes ?? ImmutableEquatableArray<ContainingTypeModel>.Empty;
            if (leftContaining.Count != rightContaining.Count)
            {
                return false;
            }

            for (int i = 0; i < leftContaining.Count; i++)
            {
                if (!string.Equals(leftContaining[i].Name, rightContaining[i].Name, StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }

        private static int GetTypeArrayHashCode(ImmutableEquatableArray<TypeModel> values)
        {
            int hash = 0;
            foreach (TypeModel value in values ?? ImmutableEquatableArray<TypeModel>.Empty)
            {
                hash = HashCode.Combine(hash, GetTypeHashCode(value));
            }

            return hash;
        }

        private static int GetTypeHashCode(TypeModel type)
        {
            int hash = HashCode.Combine(
                StringComparer.Ordinal.GetHashCode(type.Namespace ?? string.Empty),
                StringComparer.Ordinal.GetHashCode(type.Name ?? string.Empty));

            foreach (ContainingTypeModel containingType in type.ContainingTypes ?? ImmutableEquatableArray<ContainingTypeModel>.Empty)
            {
                hash = HashCode.Combine(hash, StringComparer.Ordinal.GetHashCode(containingType.Name ?? string.Empty));
            }

            return hash;
        }
    }
}
