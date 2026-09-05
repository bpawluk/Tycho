using System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models.System
{
    public readonly struct MethodInvocationModel : IEquatable<MethodInvocationModel>
    {
        public MethodSignatureModel Signature { get; }

        public TypeReferenceModel? ReceiverType { get; }

        public ImmutableEquatableArray<TypeArgumentModel> TypeArguments { get; }

        public MethodInvocationModel(
            MethodSignatureModel signature,
            TypeReferenceModel? receiverType,
            ImmutableEquatableArray<TypeArgumentModel> typeArguments)
        {
            Signature = signature;
            ReceiverType = receiverType;
            TypeArguments = typeArguments ?? ImmutableEquatableArray<TypeArgumentModel>.Empty;
        }

        public bool Equals(MethodInvocationModel other)
        {
            bool bothReceiversNull = ReceiverType is null && other.ReceiverType is null;
            return Signature.Equals(other.Signature) &&
                   (bothReceiversNull || ReceiverType?.Equals(other.ReceiverType) == true) &&
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
                ReceiverType?.GetHashCode() ?? 0,
                TypeArguments.GetHashCode());
        }

        public static bool operator ==(MethodInvocationModel left, MethodInvocationModel right) => left.Equals(right);

        public static bool operator !=(MethodInvocationModel left, MethodInvocationModel right) => !left.Equals(right);
    }
}
