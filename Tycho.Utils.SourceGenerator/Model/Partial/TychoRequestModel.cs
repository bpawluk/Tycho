using System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Model.Partial
{
    public readonly struct TychoRequestModel : IEquatable<TychoRequestModel>
    {
        public TypeModel RequestType { get; }

        public TypeModel? ResponseType { get; }

        public bool HasResponse => ResponseType.HasValue;

        public TychoRequestModel(
            TypeModel requestType, 
            TypeModel? responseType = default)
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
