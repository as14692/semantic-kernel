// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

namespace Connectors.Amazon.Models.Stability;

/// <summary>
/// Represents the response from the Stable Diffusion service.
/// </summary>
public static class StableDiffusionResponse
{
    /// <summary>
    /// Response for invoke.
    /// </summary>
    public class StableDiffusionInvokeResponse
    {
        /// <summary>
        /// The result of the operation. If successful, the response is success.
        /// </summary>
        [JsonPropertyName("result")]
        public string? Result { get; set; }
        /// <summary>
        /// An array of images, one for each requested image.
        /// </summary>
        [JsonPropertyName("artifacts")]
        public List<Artifact>? Artifacts { get; set; }
        /// <summary>
        /// An array of images, one for each requested image.
        /// </summary>
        public class Artifact
        {
            /// <summary>
            /// The value of the seed used to generate the image.
            /// </summary>
            [JsonPropertyName("seed")]
            public int Seed { get; set; }
            /// <summary>
            /// The base64 encoded image that the model generated.
            /// </summary>
            [JsonPropertyName("base64")]
            public string? Base64 { get; set; }
            /// <summary>
            /// The result of the image generation process. Valid values are:
            /// SUCCESS – The image generation process succeeded.
            /// ERROR – An error occured.
            /// CONTENT_FILTERED – The content filter filtered the image and the image might be blurred.
            /// </summary>
            [JsonPropertyName("finishReason")]
            public string? FinishReason { get; set; }
        }
    }
}
