// Licensed under the MIT License. See LICENSE in the project root for license information.

using HuoshanAI.Extensions;
using System.Text.Json.Serialization;

namespace HuoshanAI.Realtime
{
    public abstract class BaseRealtimeEvent : IRealtimeEvent
    {
        /// <inheritdoc />
        [JsonIgnore]
        public abstract string EventId { get; internal set; }

        /// <inheritdoc />
        [JsonIgnore]
        public abstract string Type { get; protected set; }

        /// <inheritdoc />
        public string ToJsonString()
            => this.ToEscapedJsonString<object>();
    }
}
