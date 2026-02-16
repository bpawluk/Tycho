using System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models.System
{
    public readonly struct MethodDefinitionModel : IEquatable<MethodDefinitionModel>
    {
        public TypeModel ContainingType { get; }

        public MethodSignatureModel Signature { get; }

        public ImmutableEquatableArray<MethodInvocationModel> Body { get; }

        public MethodDefinitionModel(
            TypeModel containingType,
            MethodSignatureModel signature,
            ImmutableEquatableArray<MethodInvocationModel> body)
        {
            ContainingType = containingType;
            Signature = signature;
            Body = body;
        }

        public bool Equals(MethodDefinitionModel other)
        {
            return ContainingType.Equals(other.ContainingType) &&
                   Signature.Equals(other.Signature) &&
                   Body.Equals(other.Body);
        }

        public override bool Equals(object obj)
        {
            return obj is MethodDefinitionModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                ContainingType.GetHashCode(),
                Signature.GetHashCode(),
                Body.GetHashCode());
        }

        public static bool operator ==(MethodDefinitionModel left, MethodDefinitionModel right) => left.Equals(right);

        public static bool operator !=(MethodDefinitionModel left, MethodDefinitionModel right) => !left.Equals(right);
    }
}
