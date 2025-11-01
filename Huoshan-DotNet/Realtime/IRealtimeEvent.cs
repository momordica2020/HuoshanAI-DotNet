// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace HuoshanAI.Realtime
{
    public interface IRealtimeEvent
    {
        /// <summary>
        /// The unique ID of the server event.
        /// </summary>
        string EventId { get; }

        string Type { get; }

        string ToJsonString();
    }

    public interface IClientEvent : IRealtimeEvent
    {
    }

    public interface IServerEvent : IRealtimeEvent
    {
    }

    internal interface IRealtimeEventStream
    {
        bool IsDone { get; }

        bool IsDelta { get; }
    }
}
