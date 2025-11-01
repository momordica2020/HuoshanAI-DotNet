// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace HuoshanAI
{
    /// <summary>
    /// Constrains the effort of reasoning for <see href="https://platform.HuoshanAI.com/docs/guides/reasoning">Reasoning Models</see>.<br/>
    /// Currently supported values are: Minimal, Low, Medium, High. Reducing reasoning effort can result in faster responses and fewer tokens used on reasoning response.
    /// </summary>
    /// <remarks>
    /// <b>Reasoning models only!</b>
    /// </remarks>
    public enum ReasoningEffort
    {
        [EnumMember(Value = "minimal")]
        Minimal = 1,
        [EnumMember(Value = "low")]
        Low,
        [EnumMember(Value = "medium")]
        Medium,
        [EnumMember(Value = "high")]
        High
    }
}
