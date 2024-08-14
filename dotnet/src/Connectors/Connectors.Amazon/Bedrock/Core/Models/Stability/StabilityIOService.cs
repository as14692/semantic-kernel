// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Amazon.BedrockRuntime.Model;
using Connectors.Amazon.Models.Stability;

namespace Microsoft.SemanticKernel.Connectors.Amazon.Core;

/// <summary>
/// StabilityIOService class for handling StabilityIO requests and responses.
/// </summary>
public class StabilityIOService : IBedrockTextToImageIOService
{
    /// <inheritdoc />
    public object GetInvokeRequestBodyForTextToImage(
        string modelId,
        string description,
        int width,
        int height,
        PromptExecutionSettings? executionSettings = null)
    {
        var requestBody = new
        {
            text_prompts = new List<object>()
            {
                new
                {
                    text = description,
                    weight = BedrockModelUtilities.GetExtensionDataValue<float?>(executionSettings?.ExtensionData, "weight")
                }
            },
            height,
            width,
            cfg_scale = BedrockModelUtilities.GetExtensionDataValue<float?>(executionSettings?.ExtensionData, "cfg_scale"),
            clip_guidance_preset = BedrockModelUtilities.GetExtensionDataValue<string?>(executionSettings?.ExtensionData, "clip_guidance_preset"),
            samples = BedrockModelUtilities.GetExtensionDataValue<int?>(executionSettings?.ExtensionData, "samples"),
            seed = BedrockModelUtilities.GetExtensionDataValue<int?>(executionSettings?.ExtensionData, "seed"),
            steps = BedrockModelUtilities.GetExtensionDataValue<int?>(executionSettings?.ExtensionData, "steps"),
            style_preset = BedrockModelUtilities.GetExtensionDataValue<string?>(executionSettings?.ExtensionData, "style_preset")
        };

        return requestBody;
    }

    /// <inheritdoc />
    public string GetInvokeResponseForImage(InvokeModelResponse response)
    {
        using var memoryStream = new MemoryStream();
        response.Body.CopyToAsync(memoryStream).ConfigureAwait(false).GetAwaiter().GetResult();
        memoryStream.Position = 0;
        using var reader = new StreamReader(memoryStream);
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
