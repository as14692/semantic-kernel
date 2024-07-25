// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;
using Connectors.Amazon.Core.Requests;

namespace Connectors.Amazon.Models.Stability;

/// <summary>
/// Stability AI Stable Diffusion request object.
/// </summary>
public static class StableDiffusionRequest
{
    /// <summary>
    /// Invoke request object
    /// </summary>
    public class StableDiffusionInvokeRequest : ITextToImageRequest
    {
        /// <summary>
        /// An array of text prompts to use for generation. Each element is a JSON object that contains a prompt and a weight for the prompt.
        /// </summary>
        [JsonPropertyName("text_prompts")]
        public List<TextPrompt> TextPrompts { get; set; }
        /// <summary>
        /// Height of the image to generate, in pixels, in an increment divible by 64.
        /// The value must be one of 1024x1024, 1152x896, 1216x832, 1344x768, 1536x640, 640x1536, 768x1344, 832x1216, 896x1152.
        /// </summary>
        [JsonPropertyName("height")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Height { get; set; }
        /// <summary>
        /// Width of the image to generate, in pixels, in an increment divible by 64.
        /// The value must be one of 1024x1024, 1152x896, 1216x832, 1344x768, 1536x640, 640x1536, 768x1344, 832x1216, 896x1152.
        /// </summary>
        [JsonPropertyName("width")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Width { get; set; }
        /// <summary>
        /// Determines how much the final image portrays the prompt. Use a lower number to increase randomness in the generation.
        /// </summary>
        [JsonPropertyName("cfg_scale")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public float? CfgScale { get; set; }
        /// <summary>
        /// Enum: FAST_BLUE, FAST_GREEN, NONE, SIMPLE SLOW, SLOWER, SLOWEST.
        /// </summary>
        [JsonPropertyName("clip_guidance_preset")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ClipGuidancePreset { get; set; }
        /// <summary>
        /// The sampler to use for the diffusion process. If this value is omitted, the model automatically selects an appropriate sampler for you.
        /// Enum: DDIM, DDPM, K_DPMPP_2M, K_DPMPP_2S_ANCESTRAL, K_DPM_2, K_DPM_2_ANCESTRAL, K_EULER, K_EULER_ANCESTRAL, K_HEUN K_LMS.
        /// </summary>
        [JsonPropertyName("sampler")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Sampler { get; set; }
        /// <summary>
        /// The number of image to generate. Currently Amazon Bedrock supports generating one image. If you supply a value for samples, the value must be one.
        /// </summary>
        [JsonPropertyName("samples")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Samples { get; set; }
        /// <summary>
        /// The seed determines the initial noise setting. Use the same seed and the same settings as a previous run to allow inference to create a similar image. If you don't set this value, or the value is 0, it is set as a random number.
        /// </summary>
        [JsonPropertyName("seed")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Seed { get; set; }
        /// <summary>
        /// Generation step determines how many times the image is sampled. More steps can result in a more accurate result.
        /// </summary>
        [JsonPropertyName("steps")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Steps { get; set; }
        /// <summary>
        /// A style preset that guides the image model towards a particular style. This list of style presets is subject to change.
        /// Enum: 3d-model, analog-film, anime, cinematic, comic-book, digital-art, enhance, fantasy-art, isometric, line-art, low-poly, modeling-compound, neon-punk, origami, photographic, pixel-art, tile-texture.
        /// </summary>
        [JsonPropertyName("style_preset")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? StylePreset { get; set; }
        /// <summary>
        /// Extra parameters passed to the engine. Use with caution. These parameters are used for in-development or experimental features and might change without warning.
        /// </summary>
        [JsonPropertyName("extras")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, object>? Extras { get; set; }
        /// <summary>
        /// An array of text prompts to use for generation. Each element is a JSON object that contains a prompt and a weight for the prompt.
        /// </summary>
        public class TextPrompt
        {
            /// <summary>
            /// The prompt that you want to pass to the model.
            /// </summary>
            [JsonPropertyName("text")]
            public string Text { get; set; }
            /// <summary>
            /// The weight that the model should apply to the prompt. A value that is less than zero declares a negative prompt. Use a negative prompt to tell the model to avoid certain concepts. The default value for weight is one.
            /// </summary>
            [JsonPropertyName("weight")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public float? Weight { get; set; }
        }
    }
}
