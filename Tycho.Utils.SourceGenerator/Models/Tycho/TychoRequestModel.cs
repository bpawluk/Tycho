using System;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models.Tycho
{
    public readonly struct TychoRequestModel : IEquatable<TychoRequestModel>
    {
        public TypeReferenceModel RequestType { get; }

        public TypeReferenceModel? ResponseType { get; }

        public bool HasResponse => ResponseType.HasValue;

        public TychoRequestModel(
            TypeReferenceModel requestType,
            TypeReferenceModel? responseType = default)
        {
            RequestType = requestType;
            ResponseType = responseType;
        }

        public bool Equals(TychoRequestModel other)
        {
            return RequestType.Equals(other.RequestType)
                && Nullable.Equals(ResponseType, other.ResponseType);
        }

        public override bool Equals(object obj)
        {
            return obj is TychoRequestModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                RequestType.GetHashCode(),
                ResponseType.GetHashCode());
        }

        public static bool operator ==(TychoRequestModel left, TychoRequestModel right) => left.Equals(right);

        public static bool operator !=(TychoRequestModel left, TychoRequestModel right) => !left.Equals(right);
    }
}
