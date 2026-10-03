using System;
using System.Text.Json;

namespace Tycho.Events.Serialization
{
    /// <summary>
    /// Defines settings for Tycho event payload serialization.
    /// </summary>
    public sealed class JsonPayloadSerializerSettings
    {
        /// <summary>
        /// Gets or sets the JSON options.
        /// </summary>
        public JsonSerializerOptions JsonOptions { get; set; } = new();

        internal void Validate()
        {
            if (JsonOptions == null)
            {
                throw new ArgumentNullException(nameof(JsonOptions));
            }
        }

        internal JsonPayloadSerializerSettings Copy()
        {
            return new()
            {
                JsonOptions = new JsonSerializerOptions(JsonOptions)
            };
        }
    }
}
