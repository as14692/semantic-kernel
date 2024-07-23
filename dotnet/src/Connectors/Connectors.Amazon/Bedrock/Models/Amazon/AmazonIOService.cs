﻿// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json;
using System.Text.Json.Nodes;
using Amazon.BedrockRuntime;
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
public class AmazonIOService : IBedrockModelIOService<IChatCompletionRequest, IChatCompletionResponse>,
    IBedrockModelIOService<ITextGenerationRequest, ITextGenerationResponse>,
    IBedrockModelIOService<ITextEmbeddingRequest, ITextEmbeddingResponse>
{
    /// <summary>
    /// Builds InvokeModel request Body parameter with structure as required by Amazon Titan.
    /// </summary>
    /// <param name="prompt">The input prompt for text generation.</param>
    /// <param name="executionSettings">Optional prompt execution settings.</param>
    /// <returns></returns>
    public object GetInvokeModelRequestBody(string prompt, PromptExecutionSettings? executionSettings = null)
    {
        double? temperature = 0.7;
        double? topP = 0.9;
        int? maxTokenCount = 512;
        List<string>? stopSequences = [];

        if (executionSettings != null && executionSettings.ExtensionData != null)
        {
            executionSettings.ExtensionData.TryGetValue("temperature", out var temperatureValue);
            temperature = temperatureValue as double?;

            executionSettings.ExtensionData.TryGetValue("top_p", out var topPValue);
            topP = topPValue as double?;

            executionSettings.ExtensionData.TryGetValue("max_tokens", out var maxTokensValue);
            maxTokenCount = maxTokensValue as int?;

            executionSettings.ExtensionData.TryGetValue("stop_sequences", out var stopSequencesValue);
            stopSequences = stopSequencesValue as List<string>;
        }
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
    public ConverseRequest GetConverseRequest(string modelId, ChatHistory chatHistory, PromptExecutionSettings settings)
    {
        var titanRequest = new TitanRequest.TitanChatCompletionRequest
        {
            Messages = chatHistory.Select(m => new Message
            {
                Role = MapRole(m.Role),
                Content = new List<ContentBlock> { new() { Text = m.Content } }
            }).ToList(),
            System = new List<SystemContentBlock>(), // { new SystemContentBlock { Text = "You are an AI assistant." } },
            InferenceConfig = new InferenceConfiguration
            {
                Temperature = this.GetExtensionDataValue(settings.ExtensionData, "temperature", 0.7f),
                TopP = this.GetExtensionDataValue(settings.ExtensionData, "topP", 0.9f),
                MaxTokens = this.GetExtensionDataValue(settings.ExtensionData, "maxTokenCount", 512),
            },
            AdditionalModelRequestFields = new Document(),
            AdditionalModelResponseFieldPaths = new List<string>()
        };
        var converseRequest = new ConverseRequest
        {
            ModelId = modelId,
            Messages = titanRequest.Messages,
            System = titanRequest.System,
            InferenceConfig = titanRequest.InferenceConfig,
            AdditionalModelRequestFields = titanRequest.AdditionalModelRequestFields,
            AdditionalModelResponseFieldPaths = titanRequest.AdditionalModelResponseFieldPaths,
            GuardrailConfig = null, // Set if needed
            ToolConfig = null // Set if needed
        };
        return converseRequest;
    }

    private TValue GetExtensionDataValue<TValue>(IDictionary<string, object>? extensionData, string key, TValue defaultValue)
    {
        if (extensionData == null || !extensionData.TryGetValue(key, out object? value))
        {
            return defaultValue;
        }

        if (value is TValue typedValue)
        {
            return typedValue;
        }

        return defaultValue;
    }

    private static ConversationRole MapRole(AuthorRole role)
    {
        string roleStr;
        if (role == AuthorRole.User)
        {
            roleStr = "user";
        }
        else
        {
            roleStr = "assistant";
        }
        return roleStr switch
        {
            "user" => ConversationRole.User,
            "assistant" => ConversationRole.Assistant,
            _ => throw new ArgumentOutOfRangeException(nameof(role), $"Invalid role: {role}")
        };
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
    public ConverseStreamRequest GetConverseStreamRequest(string modelId, ChatHistory chatHistory, PromptExecutionSettings settings)
    {
        var titanRequest = new TitanRequest.TitanChatCompletionRequest
        {
            Messages = chatHistory.Select(m => new Message
            {
                Role = MapRole(m.Role),
                Content = new List<ContentBlock> { new() { Text = m.Content } }
            }).ToList(),
            System = new List<SystemContentBlock>(), // { new SystemContentBlock { Text = "You are an AI assistant." } },
            InferenceConfig = new InferenceConfiguration
            {
                Temperature = this.GetExtensionDataValue(settings.ExtensionData, "temperature", 0.7f),
                TopP = this.GetExtensionDataValue(settings.ExtensionData, "topP", 0.9f),
                MaxTokens = this.GetExtensionDataValue(settings.ExtensionData, "maxTokenCount", 512),
            },
            AdditionalModelRequestFields = new Document(),
            AdditionalModelResponseFieldPaths = new List<string>()
        };

        var converseStreamRequest = new ConverseStreamRequest
        {
            ModelId = modelId,
            Messages = titanRequest.Messages,
            System = titanRequest.System,
            InferenceConfig = titanRequest.InferenceConfig,
            AdditionalModelRequestFields = titanRequest.AdditionalModelRequestFields,
            AdditionalModelResponseFieldPaths = titanRequest.AdditionalModelResponseFieldPaths,
            GuardrailConfig = null, // Set if needed
            ToolConfig = null // Set if needed
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
                var embedding = new ReadOnlyMemory<float>(responseBody?.Embedding.ToArray());
                return embedding;
            }
        }
    }
}
