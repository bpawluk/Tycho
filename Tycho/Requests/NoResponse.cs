using System;

namespace Tycho.Requests
{
    /// <summary>
    /// Represents the result of a request that does not return any response.
    /// </summary>
    public readonly struct NoResponse : IEquatable<NoResponse>
    {
        /// <summary>
        /// Gets the no response value.
        /// </summary>
        public static NoResponse Value => default;

        /// <inheritdoc/>
        public bool Equals(NoResponse other) => true;

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is NoResponse;

        /// <inheritdoc/>
        public override int GetHashCode() => 0;

        /// <summary>
        /// Determines whether two no response values are equal.
        /// </summary>
        public static bool operator ==(NoResponse _, NoResponse __) => true;

        /// <summary>
        /// Determines whether two no response values are not equal.
        /// </summary>
        public static bool operator !=(NoResponse _, NoResponse __) => false;

        /// <inheritdoc/>
        public override string ToString() => "()";
    }
}
