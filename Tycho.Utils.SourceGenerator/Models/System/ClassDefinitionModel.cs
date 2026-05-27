using System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models.System
{
    public readonly struct ClassDefinitionModel : IEquatable<ClassDefinitionModel>
    {
        public TypeModel ClassType { get; }

        public ImmutableEquatableArray<MethodDefinitionModel> Methods { get; }

        public ClassDefinitionModel(
            TypeModel classType,
            ImmutableEquatableArray<MethodDefinitionModel> methods)
        {
            ClassType = classType;
            Methods = methods ?? ImmutableEquatableArray<MethodDefinitionModel>.Empty;
        }

        public bool Equals(ClassDefinitionModel other)
        {
            return ClassType.Equals(other.ClassType) &&
                   Methods.Equals(other.Methods);
        }

        public override bool Equals(object obj)
        {
            return obj is ClassDefinitionModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                ClassType.GetHashCode(),
                Methods.GetHashCode());
        }

        public static bool operator ==(ClassDefinitionModel left, ClassDefinitionModel right) => left.Equals(right);

        public static bool operator !=(ClassDefinitionModel left, ClassDefinitionModel right) => !left.Equals(right);
    }
}
