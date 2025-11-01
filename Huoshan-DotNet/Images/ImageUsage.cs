// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json.Serialization;

namespace HuoshanAI.Images
{
    public sealed class ImageUsage
    {
        /// <summary>
        /// 模型成功生成的图片张数，不包含生成失败的图片。
        /// 仅对成功生成图片按张数进行计费。
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("generated_images ")]
        public int generated_images { get; private set; }


        /// <summary>
        /// 模型生成的图片花费的 token 数量。
        /// 计算逻辑为：计算sum(图片长*图片宽)/256 ，然后取整。
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("output_tokens ")]
        public int output_tokens { get; private set; }

        /// <summary>
        /// 本次请求消耗的总 token 数量。
        /// 当前不计算输入 token，故与 output_tokens 值一致。
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("total_tokens  ")]
        public int total_tokens { get; private set; }


        //public static implicit operator string(ImageError result) => result?.ToString();

        //public override string ToString()
        //{
        //    return $"{Code}:{Message}";
        //}
    }
}
