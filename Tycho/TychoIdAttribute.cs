using System;

namespace Tycho
{
    /// <summary>
    /// Assigns a stable identity to a type, independent of its CLR name and assembly.
    /// </summary>
    /// <remarks>
    /// Generic identities also include the identities of their type arguments.
    /// This attribute is not inherited by derived types.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class TychoIdAttribute : Attribute
    {
        /// <summary>
        /// Gets the explicit type identity.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Assigns an identity containing only ASCII letters, digits, dots, hyphens, and underscores.
        /// </summary>
        /// <param name="id">The nonempty, case-sensitive identity.</param>
        /// <exception cref="ArgumentException">The identity is empty or contains an unsupported character.</exception>
        public TychoIdAttribute(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("A Tycho ID cannot be empty.", nameof(id));
            }

            foreach (char character in id)
            {
                if (!(character >= 'A' && character <= 'Z') &&
                    !(character >= 'a' && character <= 'z') &&
                    !(character >= '0' && character <= '9') &&
                    character != '.' && character != '-' && character != '_')
                {
                    throw new ArgumentException("A Tycho ID can contain only ASCII letters, digits, dots, hyphens, and underscores.", nameof(id));
                }
            }

            Id = id;
        }
    }
}
