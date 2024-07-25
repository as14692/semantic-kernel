// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;
using Connectors.Amazon.Core.Responses;

namespace Connectors.Amazon.Models.Stability;

/// <summary>
/// Represents the response from the Stable Diffusion service.
/// </summary>
public static class StableDiffusionResponse
{
    /// <summary>
    /// Response for invoke.
    /// </summary>
    public class StableDiffusionInvokeResponse : ITextToImageResponse
    {
        [JsonPropertyName("result")]
        public string Result { get; set; }

        [JsonPropertyName("artifacts")]
        public List<Artifact> Artifacts { get; set; }

        public class Artifact
        {
            [JsonPropertyName("seed")]
            public int Seed { get; set; }

            [JsonPropertyName("base64")]
            public string Base64 { get; set; }

            [JsonPropertyName("finishReason")]
            public string FinishReason { get; set; }
        }
    }
}
