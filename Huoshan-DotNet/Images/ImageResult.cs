// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json.Serialization;

namespace HuoshanAI.Images
{
    public sealed class ImageResult
    {
        /// <summary>
        /// 图片的 url 信息，当 response_format 指定为 url 时返回。
        /// 该链接将在生成后 24 小时内失效，请务必及时保存图像。
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("url")]
        public string Url { get; private set; }


        /// <summary>
        /// 图片的 base64 信息，当 response_format 指定为 b64_json 时返回。
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("b64_json")]
        public string B64_Json { get; private set; }


        /// <summary>
        /// 仅 doubao-seedream-4.0 支持该字段。
        /// 图像的宽高像素值，格式<宽像素>x<高像素>，
        /// 如2048×2048。
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("size")]
        public string Size { get; internal set; }

        [JsonIgnore]
        public DateTime CreatedAt { get; internal set; }

        //[JsonIgnore]
        //public string Background { get; internal set; }

        //[JsonIgnore]
        //public string OutputFormat { get; internal set; }

        //[JsonIgnore]
        //public string Quality { get; internal set; }

        [JsonInclude]
        [JsonPropertyName("error")]
        public ImageError Error { get; internal set; }

        [JsonIgnore]
        public TokenUsage Usage { get; internal set; }

        public static implicit operator string(ImageResult result) => result?.ToString();

        public override string ToString()
            => !string.IsNullOrWhiteSpace(Url)
                ? Url
                : !string.IsNullOrWhiteSpace(B64_Json)
                    ? B64_Json
                    : string.Empty;
    }
}
