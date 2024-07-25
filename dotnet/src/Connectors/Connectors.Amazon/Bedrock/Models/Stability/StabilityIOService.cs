// Copyright (c) Microsoft. All rights reserved.

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
    private readonly BedrockModelUtilities _util = new();
    // Default values
    private const float DefaultCfgScale = 7.0f;
    private const int DefaultSamples = 1;
    private const int DefaultSeed = 0;
    private const int DefaultSteps = 30;
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
        float cfgScale = this._util.GetExtensionDataValue(executionSettings?.ExtensionData, "cfgScale", DefaultCfgScale);
        string clipGuidancePreset = this._util.GetExtensionDataValue(executionSettings?.ExtensionData, "clipGuidancePreset", string.Empty);
        string sampler = this._util.GetExtensionDataValue(executionSettings?.ExtensionData, "sampler", string.Empty);
        int samples = this._util.GetExtensionDataValue(executionSettings?.ExtensionData, "samples", DefaultSamples);
        int seed = this._util.GetExtensionDataValue(executionSettings?.ExtensionData, "seed", DefaultSeed);
        int steps = this._util.GetExtensionDataValue(executionSettings?.ExtensionData, "steps", DefaultSteps);
        string stylePreset = this._util.GetExtensionDataValue(executionSettings?.ExtensionData, "stylePreset", string.Empty);
        // Dictionary<string, object>? extras = this._util.GetExtensionDataValue(executionSettings?.ExtensionData, "extras", null);

        var requestBody = new
        {
            text_prompts = new[]
            {
                new
                {
                    text = description,
                    // weight = 1.0f
                }
            },
            height,
            width,
            // cfg_scale = cfgScale,
            // clip_guidance_preset = clipGuidancePreset,
            // sampler,
            // samples,
            // seed,
            // steps,
            // style_preset = stylePreset
        };

        return requestBody;
    }

    /// <inheritdoc />
    public string GetInvokeResponseForImage(InvokeModelResponse response)
    {
        using (var memoryStream = new MemoryStream())
        {
            response.Body.CopyToAsync(memoryStream).ConfigureAwait(false).GetAwaiter().GetResult();
            memoryStream.Position = 0;
            using (var reader = new StreamReader(memoryStream))
            {
                var responseBody = JsonSerializer.Deserialize<StableDiffusionResponse.StableDiffusionInvokeResponse>(reader.ReadToEnd());
                if (responseBody?.Artifacts != null && responseBody.Artifacts.Count > 0)
                {
                    var artifact = responseBody.Artifacts[0];
                    if (artifact.FinishReason == "SUCCESS")
                    {
                        return artifact.Base64;
                    }
                    return $"Image generation failed: {artifact.FinishReason}";
                }
                return "No image data received.";
            }
        }
    }
}
