// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Microsoft.SemanticKernel.Connectors.Amazon.Core;

internal sealed class StableRequest
{
    /// <summary>
    /// Text prompts to use for generation.
    /// </summary>
    [JsonPropertyName("text_prompts")]
    public required List<TextPrompt> TextPrompts { get; set; }

    /// <summary>
    /// Height of the image to generate, in pixels, in an increment divisible by 64.
    /// </summary>
    [JsonPropertyName("height")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Height { get; set; }

    /// <summary>
    /// Width of the image to generate, in pixels, in an increment divisible by 64.
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
    /// Enum for clip guidance preset.
    /// </summary>
    [JsonPropertyName("clip_guidance_preset")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ClipGuidancePreset { get; set; }

    /// <summary>
    /// The sampler to use for the diffusion process.
    /// </summary>
    [JsonPropertyName("sampler")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Sampler { get; set; }

    /// <summary>
    /// The number of images to generate. Currently Amazon Bedrock supports generating one image.
    /// </summary>
    [JsonPropertyName("samples")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Samples { get; set; }

    /// <summary>
    /// The seed determines the initial noise setting.
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
    /// A style preset that guides the image model towards a particular style.
    /// </summary>
    [JsonPropertyName("style_preset")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? StylePreset { get; set; }

    /// <summary>
    /// Extra parameters passed to the engine.
    /// </summary>
    [JsonPropertyName("extras")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Extras { get; set; }

    internal sealed class TextPrompt
    {
        /// <summary>
        /// The prompt that you want to pass to the model.
        /// </summary>
        [JsonPropertyName("text")]
        public required string Text { get; set; }

        /// <summary>
        /// The weight that the model should apply to the prompt.
        /// </summary>
        [JsonPropertyName("weight")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public float? Weight { get; set; }
    }
}
