using System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Model.Partial
{
    public readonly struct MethodInvocationModel : IEquatable<MethodInvocationModel>
    {
        public MethodSignatureModel Signature { get; }

        public TypeModel? ReceiverType { get; }

        public ImmutableEquatableArray<TypeParameter> TypeParameters { get; }

        public MethodInvocationModel(
            MethodSignatureModel signature,
            TypeModel? receiverType,
            ImmutableEquatableArray<TypeParameter> typeParameters)
        {
            Signature = signature;
            ReceiverType = receiverType;
            TypeParameters = typeParameters;
        }

        public bool Equals(MethodInvocationModel other)
        {
            return Signature.Equals(other.Signature) && 
                   ReceiverType.Equals(other.ReceiverType) &&
                   TypeParameters.Equals(other.TypeParameters);
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
                TypeParameters.GetHashCode());
        }

        public static bool operator ==(MethodInvocationModel left, MethodInvocationModel right) => left.Equals(right);

        public static bool operator !=(MethodInvocationModel left, MethodInvocationModel right) => !left.Equals(right);
    }
}
