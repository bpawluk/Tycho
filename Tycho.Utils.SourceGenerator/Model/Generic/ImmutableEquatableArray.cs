using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Model.Generic
{
    public static class ImmutableEquatableArray
    {
        public static ImmutableEquatableArray<T> Empty<T>()
            where T : IEquatable<T> => ImmutableEquatableArray<T>.Empty;

        public static ImmutableEquatableArray<T> ToImmutableEquatableArray<T>(this IEnumerable<T> values)
            where T : IEquatable<T> => values == null ? Empty<T>() : new ImmutableEquatableArray<T>(values);
    }

    public sealed class ImmutableEquatableArray<T> : IEquatable<ImmutableEquatableArray<T>>, IReadOnlyList<T>
        where T : IEquatable<T>
    {
        public static ImmutableEquatableArray<T> Empty { get; } = new ImmutableEquatableArray<T>(Array.Empty<T>());

        private readonly T[] _values;
        public T this[int index] => _values[index];
        public int Count => _values.Length;

        public ImmutableEquatableArray(T[] values) => _values = values;

        public ImmutableEquatableArray(IEnumerable<T> values) => _values = values.ToArray();

        public bool Equals(ImmutableEquatableArray<T> other) =>
            other != null && ((ReadOnlySpan<T>)_values).SequenceEqual(other._values);

        public override bool Equals(object obj) => obj is ImmutableEquatableArray<T> other && Equals(other);

        public override int GetHashCode()
        {
            var hash = 0;

            foreach (T value in _values)
            {
                hash = HashCode.Combine(hash, value.GetHashCode());
            }

            return hash;
        }

        public Enumerator GetEnumerator() => new Enumerator(_values);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)_values).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();

        public struct Enumerator
        {
            private readonly T[] _values;
            private int _index;

            internal Enumerator(T[] values)
            {
                _values = values;
                _index = -1;
            }

            public bool MoveNext()
            {
                var newIndex = _index + 1;

                if ((uint)newIndex < (uint)_values.Length)
                {
                    _index = newIndex;
                    return true;
                }

                return false;
            }

            public readonly T Current => _values[_index];
        }
    }
}
