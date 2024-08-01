// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Specialized;
using System.Text.Json;
using System.Text.Json.Nodes;
using Amazon.BedrockRuntime.Model;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Connectors.Amazon.Models.Stability;

/// <summary>
/// StabilityIOService class for handling StabilityIO requests and responses.
/// </summary>
public class StabilityIOService : IBedrockModelIOService
{
    // Default values
    private const float DefaultCfgScale = 7f;
    private const int DefaultSamples = 1;
    private const int DefaultSeed = 0;
    private const int DefaultSteps = 30;
    private const string DefaultClipGuidancePreset = "NONE";
    private const string DefaultStylePreset = "3d-model";
    private const float DefaultWeight = 1.0f;
    /// <summary>
    /// Builds InvokeModelRequest Body parameter to be serialized.
    /// </summary>
    /// <param name="modelId">The model ID to be used as a request parameter.</param>
    /// <param name="prompt">The input prompt for text generation.</param>
    /// <param name="executionSettings">Optional prompt execution settings.</param>
    /// <returns></returns>
    public object GetInvokeModelRequestBody(string modelId, string prompt, PromptExecutionSettings? executionSettings = null)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Extracts the test contents from the InvokeModelResponse as returned by the Bedrock API.
    /// </summary>
    /// <param name="response">The InvokeModelResponse object provided by the Bedrock InvokeModelAsync output.</param>
    /// <returns></returns>
    public IReadOnlyList<TextContent> GetInvokeResponseBody(InvokeModelResponse response)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Jurassic does not support converse.
    /// </summary>
    /// <param name="modelId">The model ID.</param>
    /// <param name="chatHistory">The messages between assistant and user.</param>
    /// <param name="settings">Optional prompt execution settings.</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public ConverseRequest GetConverseRequest(string modelId, ChatHistory chatHistory, PromptExecutionSettings? settings = null)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Jurassic does not support streaming.
    /// </summary>
    /// <param name="chunk"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IEnumerable<string> GetTextStreamOutput(JsonNode chunk)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Jurassic does not support converse (or streaming for that matter).
    /// </summary>
    /// <param name="modelId"></param>
    /// <param name="chatHistory"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public ConverseStreamRequest GetConverseStreamRequest(string modelId, ChatHistory chatHistory, PromptExecutionSettings? settings = null)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Builds the InvokeModelRequest body for text embedding generation requests.
    /// This model does not support text embedding generation currently.
    /// </summary>
    /// <param name="data">The data to be passed into the request.</param>
    /// <param name="modelId">The model for the request.</param>
    /// <returns></returns>
    public object GetEmbeddingRequestBody(string data, string modelId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Extracts the embedding floats from the invoke model Bedrock runtime action response.
    /// This model does not support text embedding generation currently.
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public ReadOnlyMemory<float> GetEmbeddingResponseBody(InvokeModelResponse response)
    {
        throw new NotImplementedException();
    }
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
                    weight = BedrockModelUtilities.GetExtensionDataValue(executionSettings?.ExtensionData, "weight", DefaultWeight)
                }
            },
            height,
            width,
            cfg_scale = BedrockModelUtilities.GetExtensionDataValue(executionSettings?.ExtensionData, "cfg_scale", DefaultCfgScale),
            clip_guidance_preset = BedrockModelUtilities.GetExtensionDataValue(executionSettings?.ExtensionData, "clip_guidance_preset", DefaultClipGuidancePreset),
            samples = BedrockModelUtilities.GetExtensionDataValue(executionSettings?.ExtensionData, "samples", DefaultSamples),
            seed = BedrockModelUtilities.GetExtensionDataValue(executionSettings?.ExtensionData, "seed", DefaultSeed),
            steps = BedrockModelUtilities.GetExtensionDataValue(executionSettings?.ExtensionData, "steps", DefaultSteps),
            style_preset = BedrockModelUtilities.GetExtensionDataValue(executionSettings?.ExtensionData, "style_preset", DefaultStylePreset)
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
        var artifact = responseBody.Artifacts[0];
        return artifact.FinishReason == "SUCCESS" ? artifact.Base64 : $"Image generation failed: {artifact.FinishReason}";
    }
}
