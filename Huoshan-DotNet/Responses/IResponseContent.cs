// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace HuoshanAI.Responses
{
    public interface IResponseContent : IServerSentEvent
    {
        public ResponseContentType Type { get; }
    }
}
