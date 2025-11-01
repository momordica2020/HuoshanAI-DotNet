// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Nodes;

namespace HuoshanAI
{
    public interface IToolCall
    {
        string CallId { get; }

        string Name { get; }

        JsonNode Arguments { get; }
    }
}
