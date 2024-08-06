﻿// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json;
using System.Text.Json.Nodes;
using Amazon.BedrockRuntime.Model;
using Amazon.Runtime.Documents;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Connectors.Amazon.Core;

/// <summary>
/// Input-output service for Anthropic Claude model.
/// </summary>
internal sealed class AnthropicIOService : IBedrockModelIOService
{
    /// <summary>
    /// Builds InvokeModel request Body parameter with structure as required by Anthropic Claude.
    /// </summary>
    /// <param name="modelId">The model ID to be used as a request parameter.</param>
    /// <param name="prompt">The input prompt for text generation.</param>
    /// <param name="executionSettings">Optional prompt execution settings.</param>
    /// <returns></returns>
    public object GetInvokeModelRequestBody(string modelId, string prompt, PromptExecutionSettings? executionSettings = null)
    {
        var maxTokensToSample = BedrockModelUtilities.GetExtensionDataValue<int?>(executionSettings?.ExtensionData, "max_tokens_to_sample");
        if (!maxTokensToSample.HasValue)
        {
            maxTokensToSample = 200; // Set the default value to 200 if it's not provided in the extension data
        }
        var requestBody = new ClaudeRequest.ClaudeTextGenerationRequest()
        {
            Prompt = $"\n\nHuman: {prompt}\n\nAssistant:",
            Temperature = BedrockModelUtilities.GetExtensionDataValue<double?>(executionSettings?.ExtensionData, "temperature"),
            MaxTokensToSample = maxTokensToSample.Value,
            StopSequences = BedrockModelUtilities.GetExtensionDataValue<IList<string>?>(executionSettings?.ExtensionData, "stop_sequences"),
            TopP = BedrockModelUtilities.GetExtensionDataValue<double?>(executionSettings?.ExtensionData, "top_p"),
            TopK = BedrockModelUtilities.GetExtensionDataValue<int?>(executionSettings?.ExtensionData, "top_k")
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
        var responseBody = JsonSerializer.Deserialize<ClaudeResponse>(reader.ReadToEnd());
        var textContents = new List<TextContent>();
        if (!string.IsNullOrEmpty(responseBody?.Completion))
        {
            textContents.Add(new TextContent(responseBody.Completion));
        }
        return textContents;
    }

    /// <summary>
    /// Builds the ConverseRequest object for the Bedrock ConverseAsync call with request parameters required by Anthropic Claude.
    /// </summary>
    /// <param name="modelId">The model ID.</param>
    /// <param name="chatHistory">The messages between assistant and user.</param>
    /// <param name="settings">Optional prompt execution settings.</param>
    /// <returns></returns>
    public ConverseRequest GetConverseRequest(string modelId, ChatHistory chatHistory, PromptExecutionSettings? settings = null)
    {
        var messages = BedrockModelUtilities.BuildMessageList(chatHistory);
        var systemMessages = BedrockModelUtilities.GetSystemMessages(chatHistory);
        var temp = BedrockModelUtilities.GetExtensionDataValue<float?>(settings?.ExtensionData, "temperature");
        var topP = BedrockModelUtilities.GetExtensionDataValue<float?>(settings?.ExtensionData, "top_p");
        var maxTokens = BedrockModelUtilities.GetExtensionDataValue<int?>(settings?.ExtensionData, "max_tokens_to_sample");
        var stopSequences = BedrockModelUtilities.GetExtensionDataValue<List<string>>(settings?.ExtensionData, "stop_sequences");

        var inferenceConfig = new InferenceConfiguration();
        BedrockModelUtilities.SetPropertyIfNotNull(() => temp, value => inferenceConfig.Temperature = value);
        BedrockModelUtilities.SetPropertyIfNotNull(() => topP, value => inferenceConfig.TopP = value);
        BedrockModelUtilities.SetPropertyIfNotNull(() => maxTokens, value => inferenceConfig.MaxTokens = value);
        BedrockModelUtilities.SetPropertyIfNotNull(() => stopSequences, value => inferenceConfig.StopSequences = value);

        var additionalModelRequestFields = new Document();
        List<ClaudeToolUse.ClaudeTool>? tools = null;
        ClaudeToolUse.ClaudeToolChoice? toolChoice = null;

        if (modelId != "anthropic.claude-instant-v1" && settings?.ExtensionData != null)
        {
            if (settings.ExtensionData.ContainsKey("tools"))
            {
                tools = BedrockModelUtilities.GetExtensionDataValue<List<ClaudeToolUse.ClaudeTool>?>(settings.ExtensionData, "tools");
            }

            if (settings.ExtensionData.ContainsKey("tool_choice"))
            {
                toolChoice = BedrockModelUtilities.GetExtensionDataValue<ClaudeToolUse.ClaudeToolChoice?>(settings.ExtensionData, "tool_choice");
            }
        }

        if (tools != null)
        {
            additionalModelRequestFields.Add(
                "tools", new Document(tools.Select(t => new Document
                {
                    { "name", t.Name },
                    { "description", t.Description },
                    { "input_schema", t.InputSchema }
                }).ToList())
            );
        }

        if (toolChoice != null)
        {
            additionalModelRequestFields.Add(
                "tool_choice", new Document
                {
                    { "type", toolChoice.Type },
                    { "name", toolChoice.Name }
                }
            );
        }

        var converseRequest = new ConverseRequest
        {
            ModelId = modelId,
            Messages = messages,
            System = systemMessages,
            InferenceConfig = inferenceConfig,
            AdditionalModelRequestFields = additionalModelRequestFields,
            AdditionalModelResponseFieldPaths = new List<string>(),
            GuardrailConfig = null, // Set if needed
            ToolConfig = null // Set if needed
        };

        return converseRequest;
    }
    /// <summary>
    /// Extracts the text generation streaming output from the Anthropic Claude response object structure.
    /// </summary>
    /// <param name="chunk"></param>
    /// <returns></returns>
    public IEnumerable<string> GetTextStreamOutput(JsonNode chunk)
    {
        var text = chunk["completion"]?.ToString();
        if (!string.IsNullOrEmpty(text))
        {
            yield return text;
        }
    }

    /// <summary>
    /// Builds the ConverseStreamRequest object for the Converse Bedrock API call, including building the Anthropic Claude Request object and mapping parameters to the ConverseStreamRequest object.
    /// </summary>
    /// <param name="modelId">The model ID.</param>
    /// <param name="chatHistory">The messages between assistant and user.</param>
    /// <param name="settings">Optional prompt execution settings.</param>
    /// <returns></returns>
    public ConverseStreamRequest GetConverseStreamRequest(string modelId, ChatHistory chatHistory, PromptExecutionSettings? settings = null)
    {
        var messages = BedrockModelUtilities.BuildMessageList(chatHistory);
        var systemMessages = BedrockModelUtilities.GetSystemMessages(chatHistory);
        var temp = BedrockModelUtilities.GetExtensionDataValue<float?>(settings?.ExtensionData, "temperature");
        var topP = BedrockModelUtilities.GetExtensionDataValue<float?>(settings?.ExtensionData, "top_p");
        var maxTokens = BedrockModelUtilities.GetExtensionDataValue<int?>(settings?.ExtensionData, "max_tokens_to_sample");
        var stopSequences = BedrockModelUtilities.GetExtensionDataValue<List<string>>(settings?.ExtensionData, "stop_sequences");

        var inferenceConfig = new InferenceConfiguration();
        BedrockModelUtilities.SetPropertyIfNotNull(() => temp, value => inferenceConfig.Temperature = value);
        BedrockModelUtilities.SetPropertyIfNotNull(() => topP, value => inferenceConfig.TopP = value);
        BedrockModelUtilities.SetPropertyIfNotNull(() => maxTokens, value => inferenceConfig.MaxTokens = value);
        BedrockModelUtilities.SetPropertyIfNotNull(() => stopSequences, value => inferenceConfig.StopSequences = value);

        var additionalModelRequestFields = new Document();
        List<ClaudeToolUse.ClaudeTool>? tools = null;
        ClaudeToolUse.ClaudeToolChoice? toolChoice = null;

        if (modelId != "anthropic.claude-instant-v1" && settings?.ExtensionData != null)
        {
            if (settings.ExtensionData.ContainsKey("tools"))
            {
                tools = BedrockModelUtilities.GetExtensionDataValue<List<ClaudeToolUse.ClaudeTool>?>(settings.ExtensionData, "tools");
            }

            if (settings.ExtensionData.ContainsKey("tool_choice"))
            {
                toolChoice = BedrockModelUtilities.GetExtensionDataValue<ClaudeToolUse.ClaudeToolChoice?>(settings.ExtensionData, "tool_choice");
            }
        }

        if (tools != null)
        {
            additionalModelRequestFields.Add(
                "tools", new Document(tools.Select(t => new Document
                {
                    { "name", t.Name },
                    { "description", t.Description },
                    { "input_schema", t.InputSchema }
                }).ToList())
            );
        }

        if (toolChoice != null)
        {
            additionalModelRequestFields.Add(
                "tool_choice", new Document
                {
                    { "type", toolChoice.Type },
                    { "name", toolChoice.Name }
                }
            );
        }

        var converseRequest = new ConverseStreamRequest
        {
            ModelId = modelId,
            Messages = messages,
            System = systemMessages,
            InferenceConfig = inferenceConfig,
            AdditionalModelRequestFields = additionalModelRequestFields,
            AdditionalModelResponseFieldPaths = new List<string>(),
            GuardrailConfig = null, // Set if needed
            ToolConfig = null // Set if needed
        };

        return converseRequest;
    }
}
