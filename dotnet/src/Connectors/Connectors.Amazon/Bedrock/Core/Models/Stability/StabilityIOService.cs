// Copyright (c) Microsoft. All rights reserved.

using System.IO;
using System.Text.Json;
using Amazon.BedrockRuntime.Model;
using Connectors.Amazon.Models.Stability;

namespace Microsoft.SemanticKernel.Connectors.Amazon.Core;

/// <summary>
/// StabilityIOService class for handling StabilityIO requests and responses.
/// </summary>
internal sealed class StabilityIOService : IBedrockTextToImageIOService
{
    /// <inheritdoc />
    public object GetInvokeRequestBodyForTextToImage(
        string modelId,
        string description,
        int width,
        int height,
        PromptExecutionSettings? executionSettings = null)
    {
        var requestBody = new StableRequest
        {
            TextPrompts =
            [
                new()
                {
                    Text = description,
                    Weight = BedrockModelUtilities.GetExtensionDataValue<float?>(executionSettings?.ExtensionData, "weight")
                }
            ],
            Height = height,
            Width = width,
            CfgScale = BedrockModelUtilities.GetExtensionDataValue<float?>(executionSettings?.ExtensionData, "cfg_scale"),
            ClipGuidancePreset = BedrockModelUtilities.GetExtensionDataValue<string?>(executionSettings?.ExtensionData, "clip_guidance_preset"),
            Samples = BedrockModelUtilities.GetExtensionDataValue<int?>(executionSettings?.ExtensionData, "samples"),
            Sampler = BedrockModelUtilities.GetExtensionDataValue<string?>(executionSettings?.ExtensionData, "sampler"),
            Seed = BedrockModelUtilities.GetExtensionDataValue<int?>(executionSettings?.ExtensionData, "seed"),
            Steps = BedrockModelUtilities.GetExtensionDataValue<int?>(executionSettings?.ExtensionData, "steps"),
            StylePreset = BedrockModelUtilities.GetExtensionDataValue<string?>(executionSettings?.ExtensionData, "style_preset")
        };

        return requestBody;
    }

    /// <inheritdoc />
    public string GetInvokeResponseForImage(InvokeModelResponse response)
    {
        using var reader = new StreamReader(response.Body);
        var responseBody = JsonSerializer.Deserialize<StableDiffusionResponse.StableDiffusionInvokeResponse>(reader.ReadToEnd());
        if (responseBody?.Artifacts is not { Count: > 0 })
        {
            return "No image data received.";
        }
        StableDiffusionResponse.StableDiffusionInvokeResponse.Artifact artifact = responseBody.Artifacts[0];
        // Check for null before using FinishReason and Base64
        if (artifact.FinishReason is null || artifact.Base64 is null)
        {
            return "Image generation failed: Invalid response.";
        }
        return artifact.FinishReason != "ERROR" ? artifact.Base64 : $"Image generation failed: {artifact.FinishReason}";
    }
}
