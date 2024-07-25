﻿// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json;
using System.Text.Json.Nodes;
using Amazon.BedrockRuntime.Model;
using Amazon.Runtime.Documents;
using Connectors.Amazon.Core.Requests;
using Connectors.Amazon.Core.Responses;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Connectors.Amazon.Models.Amazon;

/// <summary>
/// Input-output service for Amazon Titan model.
/// </summary>
public class AmazonIOService : IBedrockModelIOService
{
    private readonly BedrockModelUtilities _util = new();

    // Define constants for default values
    private const float DefaultTemperature = 0.7f;
    private const float DefaultTopP = 0.9f;
    private const int DefaultMaxTokenCount = 512;
    private static readonly List<string> DefaultStopSequences = new() { "User:" };
    /// <summary>
    /// Builds InvokeModel request Body parameter with structure as required by Amazon Titan.
    /// </summary>
    /// <param name="modelId">The model ID to be used as a request parameter.</param>
    /// <param name="prompt">The input prompt for text generation.</param>
    /// <param name="executionSettings">Optional prompt execution settings.</param>
    /// <returns></returns>
    public object GetInvokeModelRequestBody(string modelId, string prompt, PromptExecutionSettings? executionSettings = null)
    {
        float temperature = this._util.GetExtensionDataValue(executionSettings?.ExtensionData, "temperature", DefaultTemperature);
        float topP = this._util.GetExtensionDataValue(executionSettings?.ExtensionData, "topP", DefaultTopP);
        int maxTokenCount = this._util.GetExtensionDataValue(executionSettings?.ExtensionData, "maxTokenCount", DefaultMaxTokenCount);
        List<string> stopSequences = this._util.GetExtensionDataValue(executionSettings?.ExtensionData, "stopSequences", DefaultStopSequences);

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
        using (var memoryStream = new MemoryStream())
        {
            response.Body.CopyToAsync(memoryStream).ConfigureAwait(false).GetAwaiter().GetResult();
            memoryStream.Position = 0;
            using (var reader = new StreamReader(memoryStream))
            {
                var responseBody = JsonSerializer.Deserialize<TitanTextResponse>(reader.ReadToEnd());
                var textContents = new List<TextContent>();

                if (responseBody?.Results != null && responseBody.Results.Count > 0)
                {
                    string? outputText = responseBody.Results[0].OutputText;
                    textContents.Add(new TextContent(outputText));
                }
                return textContents;
            }
        }
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
        var messages = this._util.BuildMessageList(chatHistory);
        var systemMessages = this._util.GetSystemMessages(chatHistory);

        var inferenceConfig = new InferenceConfiguration
        {
            Temperature = this._util.GetExtensionDataValue(settings?.ExtensionData, "temperature", DefaultTemperature),
            TopP = this._util.GetExtensionDataValue(settings?.ExtensionData, "topP", DefaultTopP),
            MaxTokens = this._util.GetExtensionDataValue(settings?.ExtensionData, "maxTokenCount", DefaultMaxTokenCount),
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
        var messages = this._util.BuildMessageList(chatHistory);
        var systemMessages = this._util.GetSystemMessages(chatHistory);

        var inferenceConfig = new InferenceConfiguration
        {
            Temperature = this._util.GetExtensionDataValue(settings?.ExtensionData, "temperature", DefaultTemperature),
            TopP = this._util.GetExtensionDataValue(settings?.ExtensionData, "topP", DefaultTopP),
            MaxTokens = this._util.GetExtensionDataValue(settings?.ExtensionData, "maxTokenCount", DefaultMaxTokenCount),
        };

        var converseStreamRequest = new ConverseStreamRequest
        {
            ModelId = modelId,
            Messages = messages,
            System = systemMessages,
            InferenceConfig = inferenceConfig,
            AdditionalModelRequestFields = new Document(),
            AdditionalModelResponseFieldPaths = new List<string>()
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
            return new TitanRequest.TitanTextEmbeddingRequest()
            {
                InputText = data
            };
        }
        return new TitanRequest.TitanTextEmbeddingRequest()
        {
            InputText = data,
            Dimensions = 512,
            Normalize = true
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
                var responseBody = JsonSerializer.Deserialize<TitanTextEmbeddingResponse>(reader.ReadToEnd());
                var embedding = new ReadOnlyMemory<float>(responseBody?.Embedding?.ToArray());
                return embedding;
            }
        }
    }
}
