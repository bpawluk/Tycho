using System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Model.Partial
{
    public readonly struct MethodInvocationModel : IEquatable<MethodInvocationModel>
    {
        public MethodSignatureModel Signature { get; }

        public ImmutableEquatableArray<TypeModel> TypeArguments { get; }

        public MethodInvocationModel(
            MethodSignatureModel signature,
            ImmutableEquatableArray<TypeModel> typeArguments)
        {
            Signature = signature;
            TypeArguments = typeArguments;
        }

        public bool Equals(MethodInvocationModel other)
        {
            return Signature.Equals(other.Signature)
                && TypeArguments.Equals(other.TypeArguments);
        }

        public override bool Equals(object obj)
        {
            return obj is MethodInvocationModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                Signature.GetHashCode(),
                TypeArguments.GetHashCode());
        }

        public static bool operator ==(MethodInvocationModel left, MethodInvocationModel right) => left.Equals(right);

        public static bool operator !=(MethodInvocationModel left, MethodInvocationModel right) => !left.Equals(right);
    }
}
