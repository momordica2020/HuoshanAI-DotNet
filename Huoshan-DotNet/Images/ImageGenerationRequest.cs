// Licensed under the MIT License. See LICENSE in the project root for license information.

using HuoshanAI.Models;
using System.Net.NetworkInformation;
using System.Text.Json.Serialization;

namespace HuoshanAI.Images
{
    /// <summary>
    /// Creates an image given a prompt.
    /// </summary>
    public sealed class ImageGenerationRequest
    {
        /// <summary>
        /// Constructor.
        /// </summary>
       
        public ImageGenerationRequest(
            string prompt,
            Model model = null,
            int? numberOfResults = null,
            ImageResponseFormat responseFormat = 0,
            string size = null,
            bool watermark = false)
        {
            Prompt = prompt;
            Model = string.IsNullOrWhiteSpace(model?.Id) ? Models.Model.DallE_2 : model;
            ResponseFormat = responseFormat;

            Size = size;


            //if (!string.IsNullOrWhiteSpace(Style))
            //{
            //    Model = Models.Model.DallE_3;
            //}

            //if (!string.IsNullOrWhiteSpace(OutputFormat) ||
            //    OutputCompression.HasValue ||
            //    !string.IsNullOrWhiteSpace(Moderation) ||
            //    !string.IsNullOrWhiteSpace(Background))
            //{
            //    Model = Models.Model.GPT_Image_1;
            //}

            Watermark = watermark;
        }



        /// <summary>
        /// 本次请求使用模型的 Model ID 或推理接入点 (Endpoint ID)。
        /// </summary>
        [JsonPropertyName("model")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [FunctionProperty("The model to use for image generation. One of `dall-e-2`, `dall-e-3`, or `gpt-image-1`. " +
                          "Defaults to `dall-e-2` unless a parameter specific to `gpt-image-1` is used.",
            true,
            possibleValues: ["dall-e-2", "dall-e-3", "gpt-image-1"])]
        public string Model { get; }

        /// <summary>
        /// 用于生成图像的提示词，支持中英文。建议不超过300个汉字或600个英文单词。
        /// 字数过多信息容易分散，模型可能因此忽略细节，只关注重点，造成图片缺失部分元素。
        /// </summary>
        [JsonPropertyName("prompt")]
        [FunctionProperty("A text description of the desired image(s). " +
                          "The maximum length is 32000 characters for `gpt-image-1`, 1000 characters for `dall-e-2` and 4000 characters for `dall-e-3`.", true)]
        public string Prompt { get; }



        /// <summary>
        /// 输入的图片信息，支持 URL 或 Base64 编码。
        /// 其中，doubao-seedream-4.0 支持单图或多图输入（查看多图融合示例），
        /// doubao-seededit-3.0-i2 仅支持单图输入。
        /// Base64编码：请遵循此格式data:image/<图片格式>;base64,<Base64编码>。
        /// 注意 <图片格式> 需小写，
        /// 如 data:image/png;base64,<base64_image>
        ///
        /// 传入图片需要满足以下条件：
        /// 图片格式：jpeg、png
        /// 宽高比（宽/高）范围：[1 / 3, 3]
        /// 宽高长度（px） > 14
        /// 大小：不超过 10MB
        /// 总像素：不超过 6000×6000 px
        /// doubao-seedream-4.0 最多支持传入 10 张参考图。
        /// </summary>
        [JsonPropertyName("image")]
        [FunctionProperty("A text description of the desired image(s). " +
                          "The maximum length is 32000 characters for `gpt-image-1`, 1000 characters for `dall-e-2` and 4000 characters for `dall-e-3`.", true)]
        public string Image { get; }


        /// <summary>
        /// 返回的图片格式，`url` or `b64_json`
        /// </summary>
        [JsonPropertyName("response_format")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonConverter(typeof(Extensions.JsonStringEnumConverter<ImageResponseFormat>))]
        public ImageResponseFormat ResponseFormat { get; }


        /// <summary>
        /// 指定生成图像的尺寸信息，支持以下两种方式，不可混用。
        /// 方式1 | 示例：指定生成图像的分辨率，并在prompt中用自然语言描述图片宽高比、图片形状或图片用途，最终由模型判断生成图片的大小。
        /// 可选值：1K、2K、4K
        /// 方式2 | 示例：指定生成图像的宽高像素值：
        /// 默认值：2048x2048
        /// 总像素取值范围：[1280x720, 4096x4096]
        /// 宽高比取值范围：[1 / 16, 16]
        /// </summary>
        [JsonPropertyName("size")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [FunctionProperty("The size of the generated images. " +
                          "Must be one of `1024x1024`, `1536x1024` (landscape), `1024x1536` (portrait), or `auto` (default value) for `gpt-image-1`, " +
                          "one of `256x256`, `512x512`, or `1024x1024` for `dall-e-2`, and one of `1024x1024`, `1792x1024`, or `1024x1792` for `dall-e-3`.",
            possibleValues: ["256x256", "512x512", "1024x1024", "1536x1024", "1024x1536", "1792x1024", "1024x1792", "auto"])]
        public string Size { get; }


        /// <summary>
        /// 仅doubao-seedream-3.0-t2i、doubao-seededit-3.0-i2i支持该参
        /// 随机数种子，用于控制模型生成内容的随机性。取值范围为 [-1, 2147483647]。
        /// 注意
        /// 相同的请求下，模型收到不同的seed值，
        /// 如：不指定seed值或令seed取值为-1（会使用随机数替代）、或手动变更seed值，将生成不同的结果。
        /// 相同的请求下，模型收到相同的seed值，会生成类似的结果，但不保证完全一致。
        /// </summary>
        [JsonPropertyName("seed")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Seed { get; }


        /// <summary>
        /// 仅doubao-seedream-4.0支持该参数 | 查看组图输出示例控制是否关闭组图功能。
        /// 说明
        /// 组图：基于您输入的内容，生成的一组内容关联的图片。
        /// auto：自动判断模式，模型会根据用户提供的提示词自主判断是否返回组图以及组图包含的图片数量。
        /// disabled：关闭组图功能，模型只会生成一张图。
        /// </summary>
        [JsonPropertyName("sequential_image_generation")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Sequential_image_generation { get; }



        /// <summary>
        /// 是否加水印。不过似乎这个参数没啥效果说是
        /// </summary>
        [JsonPropertyName("watermark")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool Watermark { get; }

        /// <summary>
        /// 设置提示词优化功能使用的模式。doubao-seedream-4.0 默认使用标准模式对用户输入的提示词进行优化。
        /// standard：标准模式，生成内容的质量更高，耗时较长。
        /// fast：快速模式，生成内容的耗时更短，质量一般。
        /// </summary>
        [JsonPropertyName("optimize_prompt_options")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ImageOptimizeOption Optimize_prompt_options { get; }



    }
}
