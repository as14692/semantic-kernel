// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;
using Connectors.Amazon.Core.Requests;

namespace Connectors.Amazon.Models.Stability;

public class StableDiffusionRequest
{
    public class StableDiffusionInvokeRequest : ITextToImageRequest
    {
        [JsonPropertyName("text_prompts")]
        public List<TextPrompt> TextPrompts { get; set; } = new();

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("cfg_scale")]
        public float CfgScale { get; set; }

        [JsonPropertyName("clip_guidance_preset")]
        public string ClipGuidancePreset { get; set; } = string.Empty;

        [JsonPropertyName("sampler")]
        public string Sampler { get; set; } = string.Empty;

        [JsonPropertyName("samples")]
        public int Samples { get; set; }

        [JsonPropertyName("seed")]
        public int Seed { get; set; }

        [JsonPropertyName("steps")]
        public int Steps { get; set; }

        [JsonPropertyName("style_preset")]
        public string StylePreset { get; set; } = string.Empty;

        [JsonPropertyName("extras")]
        public Dictionary<string, object>? Extras { get; set; }

        public class TextPrompt
        {
            [JsonPropertyName("text")]
            public string Text { get; set; } = string.Empty;

            [JsonPropertyName("weight")]
            public float Weight { get; set; }
        }
    }
}
