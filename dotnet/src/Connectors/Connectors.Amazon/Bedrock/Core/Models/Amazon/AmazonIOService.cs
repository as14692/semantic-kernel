// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Amazon.BedrockRuntime.Model;
using Amazon.Runtime.Documents;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Microsoft.SemanticKernel.Connectors.Amazon.Core;

/// <summary>
/// Input-output service for Amazon Titan model.
/// </summary>
internal sealed class AmazonIOService : IBedrockTextGenerationIOService, IBedrockChatCompletionIOService, IBedrockTextEmbeddingIOService
{
    /// <inheritdoc/>
    public object GetInvokeModelRequestBody(string modelId, string prompt, PromptExecutionSettings? executionSettings)
    {
        var exec = AmazonTitanPromptExecutionSettings.FromExecutionSettings(executionSettings);
        var temperature = BedrockModelUtilities.GetExtensionDataValue<float?>(executionSettings?.ExtensionData, "temperature") ?? exec.Temperature;
        var topP = BedrockModelUtilities.GetExtensionDataValue<float?>(executionSettings?.ExtensionData, "topP") ?? exec.TopP;
        var maxTokenCount = BedrockModelUtilities.GetExtensionDataValue<int?>(executionSettings?.ExtensionData, "maxTokenCount") ?? exec.MaxTokenCount;
        var stopSequences = BedrockModelUtilities.GetExtensionDataValue<IList<string>?>(executionSettings?.ExtensionData, "stopSequences") ?? exec.StopSequences;

        var requestBody = new TitanRequest.TitanTextGenerationRequest()
        {
            InputText = prompt,
            TextGenerationConfig = new TitanRequest.AmazonTitanTextGenerationConfig()
            {
                MaxTokenCount = maxTokenCount,
                TopP = topP,
                Temperature = temperature,
                StopSequences = stopSequences
            }
        };
        return requestBody;
    }

    /// <inheritdoc/>
    public IReadOnlyList<TextContent> GetInvokeResponseBody(InvokeModelResponse response)
    {
        using var reader = new StreamReader(response.Body);
        var responseBody = JsonSerializer.Deserialize<TitanTextResponse>(reader.ReadToEnd());
        List<TextContent> textContents = [];
        if (responseBody?.Results is not { Count: > 0 })
        {
            return textContents;
        }
        string? outputText = responseBody.Results[0].OutputText;
        textContents.Add(new TextContent(outputText));
        return textContents;
    }

    /// <inheritdoc/>
    public ConverseRequest GetConverseRequest(string modelId, ChatHistory chatHistory, PromptExecutionSettings? settings)
    {
        var messages = BedrockModelUtilities.BuildMessageList(chatHistory);
        var systemMessages = BedrockModelUtilities.GetSystemMessages(chatHistory);

        var exec = AmazonTitanPromptExecutionSettings.FromExecutionSettings(settings);
        var temperature = BedrockModelUtilities.GetExtensionDataValue<float?>(settings?.ExtensionData, "temperature") ?? exec.Temperature;
        var topP = BedrockModelUtilities.GetExtensionDataValue<float?>(settings?.ExtensionData, "topP") ?? exec.TopP;
        var maxTokenCount = BedrockModelUtilities.GetExtensionDataValue<int?>(settings?.ExtensionData, "maxTokenCount") ?? exec.MaxTokenCount;
        var stopSequences = BedrockModelUtilities.GetExtensionDataValue<List<string>?>(settings?.ExtensionData, "stopSequences") ?? exec.StopSequences;

        var inferenceConfig = new InferenceConfiguration();
        BedrockModelUtilities.SetPropertyIfNotNull(() => temperature, value => inferenceConfig.Temperature = value);
        BedrockModelUtilities.SetPropertyIfNotNull(() => topP, value => inferenceConfig.TopP = value);
        BedrockModelUtilities.SetPropertyIfNotNull(() => maxTokenCount, value => inferenceConfig.MaxTokens = value);
        BedrockModelUtilities.SetStopSequenceIfNotNull(() => stopSequences, value => inferenceConfig.StopSequences = value);

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

    /// <inheritdoc/>
    public IEnumerable<string> GetTextStreamOutput(JsonNode chunk)
    {
        var text = chunk["outputText"]?.ToString();
        if (!string.IsNullOrEmpty(text))
        {
            yield return text;
        }
    }

    /// <inheritdoc/>
    public ConverseStreamRequest GetConverseStreamRequest(string modelId, ChatHistory chatHistory, PromptExecutionSettings? settings)
    {
        var messages = BedrockModelUtilities.BuildMessageList(chatHistory);
        var systemMessages = BedrockModelUtilities.GetSystemMessages(chatHistory);

        var exec = AmazonTitanPromptExecutionSettings.FromExecutionSettings(settings);
        var temperature = BedrockModelUtilities.GetExtensionDataValue<float?>(settings?.ExtensionData, "temperature") ?? exec.Temperature;
        var topP = BedrockModelUtilities.GetExtensionDataValue<float?>(settings?.ExtensionData, "topP") ?? exec.TopP;
        var maxTokenCount = BedrockModelUtilities.GetExtensionDataValue<int?>(settings?.ExtensionData, "maxTokenCount") ?? exec.MaxTokenCount;
        var stopSequences = BedrockModelUtilities.GetExtensionDataValue<List<string>?>(settings?.ExtensionData, "stopSequences") ?? exec.StopSequences;

        var inferenceConfig = new InferenceConfiguration();
        BedrockModelUtilities.SetPropertyIfNotNull(() => temperature, value => inferenceConfig.Temperature = value);
        BedrockModelUtilities.SetPropertyIfNotNull(() => topP, value => inferenceConfig.TopP = value);
        BedrockModelUtilities.SetPropertyIfNotNull(() => maxTokenCount, value => inferenceConfig.MaxTokens = value);
        BedrockModelUtilities.SetStopSequenceIfNotNull(() => stopSequences, value => inferenceConfig.StopSequences = value);

        var converseRequest = new ConverseStreamRequest()
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
        using (var reader = new StreamReader(response.Body))
        {
            var responseBody = JsonSerializer.Deserialize<TitanEmbeddingResponse>(reader.ReadToEnd());
            if (responseBody?.Embedding is { Count: > 0 } embedding)
            {
                return new ReadOnlyMemory<float>(embedding.ToArray());
            }

            return ReadOnlyMemory<float>.Empty;
        }
    }
}
