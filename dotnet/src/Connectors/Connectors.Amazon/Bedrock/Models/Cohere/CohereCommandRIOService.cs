﻿// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json;
using System.Text.Json.Nodes;
using Amazon.BedrockRuntime.Model;
using Amazon.Runtime.Documents;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Connectors.Amazon.Models.Cohere;

/// <summary>
/// Input-output service for Cohere Command R.
/// </summary>
public class CohereCommandRIOService : IBedrockModelIOService
{
    private readonly BedrockUtilities _util = new BedrockUtilities();
    /// <summary>
    /// Builds InvokeModel request Body parameter with structure as required by Cohere Command R.
    /// </summary>
    /// <param name="prompt">The input prompt for text generation.</param>
    /// <param name="executionSettings">Optional prompt execution settings.</param>
    /// <returns></returns>
    public object GetInvokeModelRequestBody(string prompt, PromptExecutionSettings? executionSettings = null)
    {
        double? temperature = 0.3; // Cohere default
        double? topP = 0.75; // Cohere default
        int? maxTokens = null; // Cohere default
        List<string>? stopSequences = null;
        double? topK = 0; // Cohere default
        string? promptTruncation = "OFF"; // Cohere default
        double? frequencyPenalty = 0; // Cohere default
        double? presencePenalty = 0; // Cohere default
        int? seed = null;
        bool? returnPrompt = false; // Cohere default
        List<CommandRTextRequest.Tool>? tools = null;
        List<CommandRTextRequest.ToolResult>? toolResults = null;
        bool? rawPrompting = false; // Cohere default

        if (executionSettings is { ExtensionData: not null })
        {
            executionSettings.ExtensionData.TryGetValue("temperature", out var temperatureValue);
            temperature = temperatureValue as double?;

            executionSettings.ExtensionData.TryGetValue("p", out var topPValue);
            topP = topPValue as double?;

            executionSettings.ExtensionData.TryGetValue("k", out var topKValue);
            topK = topKValue as double?;

            executionSettings.ExtensionData.TryGetValue("max_tokens", out var maxTokensValue);
            maxTokens = maxTokensValue as int?;

            executionSettings.ExtensionData.TryGetValue("stop_sequences", out var stopSequencesValue);
            stopSequences = stopSequencesValue as List<string>;

            executionSettings.ExtensionData.TryGetValue("prompt_truncation", out var promptTruncationValue);
            promptTruncation = promptTruncationValue as string;

            executionSettings.ExtensionData.TryGetValue("frequency_penalty", out var frequencyPenaltyValue);
            frequencyPenalty = frequencyPenaltyValue as double?;

            executionSettings.ExtensionData.TryGetValue("presence_penalty", out var presencePenaltyValue);
            presencePenalty = presencePenaltyValue as double?;

            executionSettings.ExtensionData.TryGetValue("seed", out var seedValue);
            seed = seedValue as int?;

            executionSettings.ExtensionData.TryGetValue("return_prompt", out var returnPromptValue);
            returnPrompt = returnPromptValue as bool?;

            executionSettings.ExtensionData.TryGetValue("tools", out var toolsValue);
            tools = toolsValue as List<CommandRTextRequest.Tool>;

            executionSettings.ExtensionData.TryGetValue("tool_results", out var toolResultsValue);
            toolResults = toolResultsValue as List<CommandRTextRequest.ToolResult>;

            executionSettings.ExtensionData.TryGetValue("raw_prompting", out var rawPromptingValue);
            rawPrompting = rawPromptingValue as bool?;
        }

        var requestBody = new CommandRTextRequest.CommandRTextGenerationRequest
        {
            Message = prompt,
            Temperature = temperature,
            TopP = topP,
            TopK = topK,
            MaxTokens = maxTokens,
            StopSequences = stopSequences,
            PromptTruncation = promptTruncation,
            FrequencyPenalty = frequencyPenalty,
            PresencePenalty = presencePenalty,
            Seed = seed,
            ReturnPrompt = returnPrompt,
            Tools = tools,
            ToolResults = toolResults,
            RawPrompting = rawPrompting
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
                var responseBody = JsonSerializer.Deserialize<CommandRTextResponse>(reader.ReadToEnd());
                var textContents = new List<TextContent>();

                if (!string.IsNullOrEmpty(responseBody?.Text))
                {
                    textContents.Add(new TextContent(responseBody.Text));
                }

                return textContents;
            }
        }
    }
    /// <summary>
    /// Builds the ConverseRequest object for the Bedrock ConverseAsync call with request parameters required by Cohere Command R.
    /// </summary>
    /// <param name="modelId">The model ID</param>
    /// <param name="chatHistory">The messages between assistant and user.</param>
    /// <param name="settings">Optional prompt execution settings.</param>
    /// <returns></returns>
    public ConverseRequest GetConverseRequest(string modelId, ChatHistory chatHistory, PromptExecutionSettings? settings = null)
    {
        var cohereRequest = new CohereCommandRequest
        {
            ChatHistory = chatHistory.Select(m => new CohereCommandRequest.CohereMessage
            {
                Role = new BedrockUtilities().MapRole(m.Role),
                Message = m.Content
            }).ToList(),
            Messages = chatHistory.Select(m => new Message
            {
                Role = new BedrockUtilities().MapRole(m.Role),
                Content = new List<ContentBlock> { new() { Text = m.Content } }
            }).ToList(),
            Temperature = this._util.GetExtensionDataValue(settings?.ExtensionData, "temperature", 0.3f),
            TopP = this._util.GetExtensionDataValue(settings?.ExtensionData, "p", 0.75f),
            TopK = this._util.GetExtensionDataValue(settings?.ExtensionData, "k", 0.0f),
            MaxTokens = this._util.GetExtensionDataValue(settings?.ExtensionData, "max_tokens", 512),
            PromptTruncation = this._util.GetExtensionDataValue<string>(settings?.ExtensionData, "prompt_truncation", "OFF"),
            FrequencyPenalty = this._util.GetExtensionDataValue(settings?.ExtensionData, "frequency_penalty", 0.0),
            PresencePenalty = this._util.GetExtensionDataValue(settings?.ExtensionData, "presence_penalty", 0.0),
            Seed = this._util.GetExtensionDataValue(settings?.ExtensionData, "seed", 0),
            ReturnPrompt = this._util.GetExtensionDataValue(settings?.ExtensionData, "return_prompt", false),
            Tools = this._util.GetExtensionDataValue<List<CohereCommandRequest.CohereTool>>(settings?.ExtensionData, "tools", null),
            ToolResults = this._util.GetExtensionDataValue<List<CohereCommandRequest.CohereToolResult>>(settings?.ExtensionData, "tool_results", null),
            StopSequences = this._util.GetExtensionDataValue<List<string>>(settings?.ExtensionData, "stop_sequences", null),
            RawPrompting = this._util.GetExtensionDataValue(settings?.ExtensionData, "raw_prompting", false)
        };
        var converseRequest = new ConverseRequest
        {
            ModelId = modelId,
            Messages = cohereRequest.Messages,
            System = cohereRequest.System,
            InferenceConfig = new InferenceConfiguration
            {
                Temperature = (float)cohereRequest.Temperature,
                TopP = (float)cohereRequest.TopP,
                MaxTokens = cohereRequest.MaxTokens
            },
            AdditionalModelRequestFields = new Document
            {
                { "message", cohereRequest.Message },
                { "documents", new Document(cohereRequest.Documents?.Select(d => new Document
                {
                    { "title", d.Title },
                    { "snippet", d.Snippet }
                }).ToList() ?? new List<Document>()) },
                { "search_queries_only", cohereRequest.SearchQueriesOnly },
                { "preamble", cohereRequest.Preamble },
                { "k", cohereRequest.TopK },
                { "prompt_truncation", cohereRequest.PromptTruncation },
                { "frequency_penalty", cohereRequest.FrequencyPenalty },
                { "presence_penalty", cohereRequest.PresencePenalty },
                { "seed", cohereRequest.Seed },
                { "return_prompt", cohereRequest.ReturnPrompt },
                { "stop_sequences", new Document(cohereRequest.StopSequences?.Select(s => new Document(s)).ToList() ?? new List<Document>()) },
                { "raw_prompting", cohereRequest.RawPrompting }
            },
            AdditionalModelResponseFieldPaths = new List<string>(),
            GuardrailConfig = null,
            ToolConfig = null
        };

        return converseRequest;
    }
    /// <summary>
    /// Extracts the text generation streaming output from the Cohere Command R response object structure.
    /// </summary>
    /// <param name="chunk"></param>
    /// <returns></returns>
    public IEnumerable<string> GetTextStreamOutput(JsonNode chunk)
    {
        var text = chunk["text"]?.ToString();
        if (!string.IsNullOrEmpty(text))
        {
            yield return text;
        }
    }
    /// <summary>
    /// Builds the ConverseStreamRequest object for the Converse Bedrock API call, including building the Cohere Command R Request object and mapping parameters to the ConverseStreamRequest object.
    /// </summary>
    /// <param name="modelId">The model ID.</param>
    /// <param name="chatHistory">The messages between assistant and user.</param>
    /// <param name="settings">Optional prompt execution settings.</param>
    /// <returns></returns>
    public ConverseStreamRequest GetConverseStreamRequest(string modelId, ChatHistory chatHistory, PromptExecutionSettings? settings = null)
    {
        var cohereRequest = new CohereCommandRequest
        {
            ChatHistory = chatHistory.Select(m => new CohereCommandRequest.CohereMessage
            {
                Role = new BedrockUtilities().MapRole(m.Role),
                Message = m.Content
            }).ToList(),
            Messages = chatHistory.Select(m => new Message
            {
                Role = new BedrockUtilities().MapRole(m.Role),
                Content = new List<ContentBlock> { new() { Text = m.Content } }
            }).ToList(),
            Temperature = this._util.GetExtensionDataValue(settings?.ExtensionData, "temperature", 0.3f),
            TopP = this._util.GetExtensionDataValue(settings?.ExtensionData, "p", 0.75f),
            TopK = this._util.GetExtensionDataValue(settings?.ExtensionData, "k", 0.0f),
            MaxTokens = this._util.GetExtensionDataValue(settings?.ExtensionData, "max_tokens", 512),
            PromptTruncation = this._util.GetExtensionDataValue<string>(settings?.ExtensionData, "prompt_truncation", "OFF"),
            FrequencyPenalty = this._util.GetExtensionDataValue(settings?.ExtensionData, "frequency_penalty", 0.0),
            PresencePenalty = this._util.GetExtensionDataValue(settings?.ExtensionData, "presence_penalty", 0.0),
            Seed = this._util.GetExtensionDataValue(settings?.ExtensionData, "seed", 0),
            ReturnPrompt = this._util.GetExtensionDataValue(settings?.ExtensionData, "return_prompt", false),
            Tools = this._util.GetExtensionDataValue<List<CohereCommandRequest.CohereTool>>(settings?.ExtensionData, "tools", null),
            ToolResults = this._util.GetExtensionDataValue<List<CohereCommandRequest.CohereToolResult>>(settings?.ExtensionData, "tool_results", null),
            StopSequences = this._util.GetExtensionDataValue<List<string>>(settings?.ExtensionData, "stop_sequences", null),
            RawPrompting = this._util.GetExtensionDataValue(settings?.ExtensionData, "raw_prompting", false)
        };
        var converseRequest = new ConverseStreamRequest
        {
            ModelId = modelId,
            Messages = cohereRequest.Messages,
            System = cohereRequest.System,
            InferenceConfig = new InferenceConfiguration
            {
                Temperature = (float)cohereRequest.Temperature,
                TopP = (float)cohereRequest.TopP,
                MaxTokens = cohereRequest.MaxTokens
            },
            AdditionalModelRequestFields = new Document
            {
                { "message", cohereRequest.Message },
                {
                    "documents", new Document(cohereRequest.Documents?.Select(d => new Document
                    {
                        { "title", d.Title },
                        { "snippet", d.Snippet }
                    }).ToList() ?? new List<Document>())
                },
                { "search_queries_only", cohereRequest.SearchQueriesOnly },
                { "preamble", cohereRequest.Preamble },
                { "k", cohereRequest.TopK },
                { "prompt_truncation", cohereRequest.PromptTruncation },
                { "frequency_penalty", cohereRequest.FrequencyPenalty },
                { "presence_penalty", cohereRequest.PresencePenalty },
                { "seed", cohereRequest.Seed },
                { "return_prompt", cohereRequest.ReturnPrompt },
                { "stop_sequences", new Document(cohereRequest.StopSequences?.Select(s => new Document(s)).ToList() ?? new List<Document>()) },
                { "raw_prompting", cohereRequest.RawPrompting }
            },
            AdditionalModelResponseFieldPaths = new List<string>(),
            GuardrailConfig = null,
            ToolConfig = null
        };
        return converseRequest;
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
    /// This model does not support text embedding generation currently. Cohere has a separate embed model.
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public ReadOnlyMemory<float> GetEmbeddingResponseBody(InvokeModelResponse response)
    {
        throw new NotImplementedException();
    }
}
