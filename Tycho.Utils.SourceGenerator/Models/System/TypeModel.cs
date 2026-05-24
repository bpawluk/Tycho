using System;
using System.Linq;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models.System
{
    public readonly struct TypeModel : IEquatable<TypeModel>
    {
        public string Namespace { get; }

        public ImmutableEquatableArray<ContainingTypeModel> ContainingTypes { get; }

        public string Name { get; }

        public ImmutableEquatableArray<string> TypeParameters { get; }

        public ImmutableEquatableArray<string> TypeParameterConstraintClauses { get; }

        public ImmutableEquatableArray<string> TypeArguments { get; }

        public string TypeParametersSuffix => BuildTypeSuffix(TypeParameters);

        public string TypeArgumentsSuffix => BuildTypeSuffix(TypeArguments.Count == 0 ? TypeParameters : TypeArguments);

        public string DeclarationName => $"{Name}{TypeParametersSuffix}";

        public string ReferenceName => $"{Name}{TypeArgumentsSuffix}";

        public string MetadataName => TypeParameters.Count > 0 ? $"{Name}`{TypeParameters.Count}" : Name;

        public ImmutableEquatableArray<string> ContainingTypeDeclarationSignatures => ContainingTypes
            .Select(type => type.DeclarationSignature)
            .ToImmutableEquatableArray();

        public string PathName => BuildPath(
            ContainingTypes.Select(type => type.ReferenceName).ToImmutableEquatableArray(),
            ReferenceName);

        public string HintName => BuildPath(
            ContainingTypes.Select(type => type.MetadataName).ToImmutableEquatableArray(),
            MetadataName,
            Namespace);

        public TypeModel(string typeNamespace, string typeName)
            : this(
                typeNamespace,
                ImmutableEquatableArray<ContainingTypeModel>.Empty,
                typeName,
                ImmutableEquatableArray<string>.Empty,
                ImmutableEquatableArray<string>.Empty,
                ImmutableEquatableArray<string>.Empty)
        {
        }

        public TypeModel(
            string typeNamespace,
            ImmutableEquatableArray<ContainingTypeModel> containingTypes,
            string typeName,
            ImmutableEquatableArray<string> typeParameters,
            ImmutableEquatableArray<string> typeParameterConstraintClauses,
            ImmutableEquatableArray<string> typeArguments)
        {
            Namespace = typeNamespace ?? string.Empty;
            ContainingTypes = containingTypes ?? ImmutableEquatableArray<ContainingTypeModel>.Empty;
            Name = typeName;
            TypeParameters = typeParameters ?? ImmutableEquatableArray<string>.Empty;
            TypeParameterConstraintClauses = typeParameterConstraintClauses ?? ImmutableEquatableArray<string>.Empty;
            TypeArguments = typeArguments ?? ImmutableEquatableArray<string>.Empty;
        }

        public bool Equals(TypeModel other)
        {
            return string.Equals(Namespace, other.Namespace, StringComparison.Ordinal)
                && ContainingTypes.Equals(other.ContainingTypes)
                && string.Equals(Name, other.Name, StringComparison.Ordinal)
                && TypeParameters.Equals(other.TypeParameters)
                && TypeParameterConstraintClauses.Equals(other.TypeParameterConstraintClauses)
                && TypeArguments.Equals(other.TypeArguments);
        }

        public override bool Equals(object obj)
        {
            return obj is TypeModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                StringComparer.Ordinal.GetHashCode(Namespace ?? string.Empty),
                ContainingTypes.GetHashCode(),
                StringComparer.Ordinal.GetHashCode(Name ?? string.Empty),
                TypeParameters.GetHashCode(),
                TypeParameterConstraintClauses.GetHashCode(),
                TypeArguments.GetHashCode());
        }

        public override string ToString() => string.IsNullOrEmpty(Namespace) ? PathName : $"{Namespace}.{PathName}";

        public static bool operator ==(TypeModel left, TypeModel right) => left.Equals(right);

        public static bool operator !=(TypeModel left, TypeModel right) => !left.Equals(right);

        private static string BuildTypeSuffix(ImmutableEquatableArray<string> values)
        {
            return values.Count == 0 ? string.Empty : $"<{string.Join(", ", values)}>";
        }

        private static string BuildPath(ImmutableEquatableArray<string> containingSegments, string leafSegment, string prefix = null)
        {
            string containingPath = containingSegments.Count == 0
                ? string.Empty
                : string.Join(".", containingSegments.Where(segment => !string.IsNullOrWhiteSpace(segment)));

            string path = string.IsNullOrEmpty(containingPath) ? leafSegment : $"{containingPath}.{leafSegment}";
            return string.IsNullOrEmpty(prefix) ? path : $"{prefix}.{path}";
        }
    }
}
