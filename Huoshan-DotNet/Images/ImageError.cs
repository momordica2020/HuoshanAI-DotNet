// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json.Serialization;

namespace HuoshanAI.Images
{
    public sealed class ImageError
    {
        /// <summary>
        /// 图片的 url 信息，当 response_format 指定为 url 时返回。
        /// 该链接将在生成后 24 小时内失效，请务必及时保存图像。
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("code")]
        public string Code { get; private set; }


        /// <summary>
        /// 图片的 base64 信息，当 response_format 指定为 b64_json 时返回。
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("message")]
        public string Message { get; private set; }


        public static implicit operator string(ImageError result) => result?.ToString();

        public override string ToString()
        {
            return $"{Code}:{Message}";
        }
    }
}
