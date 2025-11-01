// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace HuoshanAI.Images
{
    /// <summary>
    /// 提示词优化功能的配置。
    /// 仅doubao-seedream-4.0支持该参数。
    /// </summary>
    public enum ImageOptimizeOption
    {
        /// <summary>
        /// 标准模式，生成内容的质量更高，耗时较长
        /// </summary>
        [EnumMember(Value = "standard")]
        Standard = 1,

        /// <summary>
        /// 快速模式，生成内容的耗时更短，质量一般
        /// </summary>
        [EnumMember(Value = "fast")]
        Fast
    }
}
