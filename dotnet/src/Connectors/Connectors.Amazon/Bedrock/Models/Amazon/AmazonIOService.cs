﻿// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json;
using System.Text.Json.Nodes;
using Amazon.BedrockRuntime.Model;
using Amazon.Runtime.Documents;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Connectors.Amazon.Models.Amazon;

/// <summary>
/// Input-output service for Amazon Titan model.
/// </summary>
public class AmazonIOService : IBedrockModelIOService
{
    // Define constants for default values
    private const float DefaultTemperature = 0.7f;
    private const float DefaultTopP = 0.9f;
    private const int DefaultMaxTokenCount = 512;
    private static readonly List<string> s_defaultStopSequences = new() { "User:" };
    /// <summary>
    /// Builds InvokeModel request Body parameter with structure as required by Amazon Titan.
    /// </summary>
    /// <param name="modelId">The model ID to be used as a request parameter.</param>
    /// <param name="prompt">The input prompt for text generation.</param>
    /// <param name="executionSettings">Optional prompt execution settings.</param>
    /// <returns></returns>
    public object GetInvokeModelRequestBody(string modelId, string prompt, PromptExecutionSettings? executionSettings = null)
    {
        float temperature = BedrockModelUtilities.GetExtensionDataValue(executionSettings?.ExtensionData, "temperature", DefaultTemperature);
        float topP = BedrockModelUtilities.GetExtensionDataValue(executionSettings?.ExtensionData, "topP", DefaultTopP);
        int maxTokenCount = BedrockModelUtilities.GetExtensionDataValue(executionSettings?.ExtensionData, "maxTokenCount", DefaultMaxTokenCount);
        List<string> stopSequences = BedrockModelUtilities.GetExtensionDataValue(executionSettings?.ExtensionData, "stopSequences", s_defaultStopSequences);

        var requestBody = new
        {
            inputText = prompt,
            textGenerationConfig = new
            {
                temperature,
                topP,
                maxTokenCount,
                stopSequences
            }
        };
        return requestBody;
    }
    /// <summary>
    /// Extracts the test contents from the InvokeModelResponse as returned by the Bedrock API.
    /// </summary>
    /// <param name="response">The InvokeModelResponse object provided by the Bedrock InvokeModelAsync output.</param>
    /// <returns></returns>
    public IReadOnlyList<TextContent> GetInvokeResponseBody(InvokeModelResponse response)
    {
        using var memoryStream = new MemoryStream();
        response.Body.CopyToAsync(memoryStream).ConfigureAwait(false).GetAwaiter().GetResult();
        memoryStream.Position = 0;
        using var reader = new StreamReader(memoryStream);
        var responseBody = JsonSerializer.Deserialize<TitanTextResponse>(reader.ReadToEnd());
        var textContents = new List<TextContent>();
        if (responseBody?.Results is not { Count: > 0 })
        {
            return textContents;
        }
        string? outputText = responseBody.Results[0].OutputText;
        textContents.Add(new TextContent(outputText));
        return textContents;
    }
    /// <summary>
    /// Builds the ConverseRequest object for the Bedrock ConverseAsync call with request parameters required by Amazon Titan.
    /// </summary>
    /// <param name="modelId">The model ID.</param>
    /// <param name="chatHistory">The messages between assistant and user.</param>
    /// <param name="settings">Optional prompt execution settings.</param>
    /// <returns></returns>
    public ConverseRequest GetConverseRequest(string modelId, ChatHistory chatHistory, PromptExecutionSettings? settings = null)
    {
        var messages = BedrockModelUtilities.BuildMessageList(chatHistory);
        var systemMessages = BedrockModelUtilities.GetSystemMessages(chatHistory);

        var inferenceConfig = new InferenceConfiguration
        {
            Temperature = BedrockModelUtilities.GetExtensionDataValue(settings?.ExtensionData, "temperature", DefaultTemperature),
            TopP = BedrockModelUtilities.GetExtensionDataValue(settings?.ExtensionData, "topP", DefaultTopP),
            MaxTokens = BedrockModelUtilities.GetExtensionDataValue(settings?.ExtensionData, "maxTokenCount", DefaultMaxTokenCount),
        };

        var converseRequest = new ConverseRequest
        {
            ModelId = modelId,
            Messages = messages,
            System = systemMessages,
            InferenceConfig = inferenceConfig,
            AdditionalModelRequestFields = new Document(),
            AdditionalModelResponseFieldPaths = new List<string>()
        };

        return converseRequest;
    }
    /// <summary>
    /// Extracts the text generation streaming output from the Amazon Titan response object structure.
    /// </summary>
    /// <param name="chunk"></param>
    /// <returns></returns>
    public IEnumerable<string> GetTextStreamOutput(JsonNode chunk)
    {
        var text = chunk["outputText"]?.ToString();
        if (!string.IsNullOrEmpty(text))
        {
            yield return text;
        }
    }
    /// <summary>
    /// Builds the ConverseStreamRequest object for the Converse Bedrock API call, including building the Amazon Titan Request object and mapping parameters to the ConverseStreamRequest object.
    /// </summary>
    /// <param name="modelId">The model ID.</param>
    /// <param name="chatHistory">The messages between assistant and user.</param>
    /// <param name="settings">Optional prompt execution settings.</param>
    /// <returns></returns>
    public ConverseStreamRequest GetConverseStreamRequest(string modelId, ChatHistory chatHistory, PromptExecutionSettings? settings = null)
    {
        var messages = BedrockModelUtilities.BuildMessageList(chatHistory);
        var systemMessages = BedrockModelUtilities.GetSystemMessages(chatHistory);

        var inferenceConfig = new InferenceConfiguration
        {
            Temperature = BedrockModelUtilities.GetExtensionDataValue(settings?.ExtensionData, "temperature", DefaultTemperature),
            TopP = BedrockModelUtilities.GetExtensionDataValue(settings?.ExtensionData, "topP", DefaultTopP),
            MaxTokens = BedrockModelUtilities.GetExtensionDataValue(settings?.ExtensionData, "maxTokenCount", DefaultMaxTokenCount),
        };

        var converseStreamRequest = new ConverseStreamRequest
        {
            ModelId = modelId,
            Messages = messages,
            System = systemMessages,
            InferenceConfig = inferenceConfig,
            AdditionalModelRequestFields = new Document(),
            AdditionalModelResponseFieldPaths = []
        };

        return converseStreamRequest;
    }
    /// <summary>
    /// Builds the InvokeModelRequest body for text embedding generation requests.
    /// </summary>
    /// <param name="data">The data to be passed into the request.</param>
    /// <param name="modelId">The model to be used for the request.</param>
    /// <returns></returns>
    public object GetEmbeddingRequestBody(string data, string modelId)
    {
        if (modelId.Contains("v1"))
        {
            return new
            {
                inputText = data
            };
        }
        return new
        {
            inputText = data,
            dimensions = 512,
            normalize = true
        };
    }
    /// <summary>
    /// Extracts the embedding floats from the invoke model Bedrock runtime action response.
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public ReadOnlyMemory<float> GetEmbeddingResponseBody(InvokeModelResponse response)
    {
        using (var memoryStream = new MemoryStream())
        {
            response.Body.CopyToAsync(memoryStream).ConfigureAwait(false).GetAwaiter().GetResult();
            memoryStream.Position = 0;
            using (var reader = new StreamReader(memoryStream))
            {
                var responseBody = JsonSerializer.Deserialize<TitanEmbeddingResponse>(reader.ReadToEnd());
                var embedding = new ReadOnlyMemory<float>(responseBody?.Embedding?.ToArray());
                return embedding;
            }
        }
    }

    /// <inheritdoc />
    /// Not supported by this model.
    public object GetInvokeRequestBodyForTextToImage(
        string modelId,
        string description,
        int width,
        int height,
        PromptExecutionSettings? executionSettings = null)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    /// Not supported by this model.
    public string GetInvokeResponseForImage(InvokeModelResponse response)
    {
        throw new NotImplementedException();
    }
}
