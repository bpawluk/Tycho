using System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models.System
{
    public readonly struct MethodInvocationModel : IEquatable<MethodInvocationModel>
    {
        public MethodSignatureModel Signature { get; }

        public TypeModel? ReceiverType { get; }

        public ImmutableEquatableArray<TypeArgument> TypeArguments { get; }

        public MethodInvocationModel(
            MethodSignatureModel signature,
            TypeModel? receiverType,
            ImmutableEquatableArray<TypeArgument> typeArguments)
        {
            Signature = signature;
            ReceiverType = receiverType;
            TypeArguments = typeArguments;
        }

        public bool Equals(MethodInvocationModel other)
        {
            return Signature.Equals(other.Signature) &&
                   ReceiverType.Equals(other.ReceiverType) &&
                   TypeArguments.Equals(other.TypeArguments);
        }

        public override bool Equals(object obj)
        {
            return obj is MethodInvocationModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                Signature.GetHashCode(),
                ReceiverType.GetHashCode(),
                TypeArguments.GetHashCode());
        }

        public static bool operator ==(MethodInvocationModel left, MethodInvocationModel right) => left.Equals(right);

        public static bool operator !=(MethodInvocationModel left, MethodInvocationModel right) => !left.Equals(right);
    }
}
