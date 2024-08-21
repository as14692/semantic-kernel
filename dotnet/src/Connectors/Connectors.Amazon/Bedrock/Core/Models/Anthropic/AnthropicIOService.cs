// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Amazon.BedrockRuntime.Model;
using Amazon.Runtime.Documents;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Microsoft.SemanticKernel.Connectors.Amazon.Core;

/// <summary>
/// Input-output service for Anthropic Claude model.
/// </summary>
internal sealed class AnthropicIOService : IBedrockTextGenerationIOService, IBedrockChatCompletionIOService
{
    /// <inheritdoc/>
    public object GetInvokeModelRequestBody(string modelId, string prompt, PromptExecutionSettings? executionSettings)
    {
        var exec = AmazonClaudePromptExecutionSettings.FromExecutionSettings(executionSettings);
        var requestBody = new ClaudeRequest.ClaudeTextGenerationRequest()
        {
            Prompt = $"\n\nHuman: {prompt}\n\nAssistant:",
            Temperature = BedrockModelUtilities.GetExtensionDataValue<double?>(executionSettings?.ExtensionData, "temperature") ?? exec.Temperature,
            MaxTokensToSample = BedrockModelUtilities.GetExtensionDataValue<int?>(executionSettings?.ExtensionData, "max_tokens_to_sample") ?? exec.MaxTokensToSample,
            StopSequences = BedrockModelUtilities.GetExtensionDataValue<IList<string>?>(executionSettings?.ExtensionData, "stop_sequences") ?? exec.StopSequences,
            TopP = BedrockModelUtilities.GetExtensionDataValue<double?>(executionSettings?.ExtensionData, "top_p") ?? exec.TopP,
            TopK = BedrockModelUtilities.GetExtensionDataValue<int?>(executionSettings?.ExtensionData, "top_k") ?? exec.TopK
        };
        return requestBody;
    }

    /// <inheritdoc/>
    public IReadOnlyList<TextContent> GetInvokeResponseBody(InvokeModelResponse response)
    {
        using var reader = new StreamReader(response.Body);
        var responseBody = JsonSerializer.Deserialize<ClaudeResponse>(reader.ReadToEnd());
        List<TextContent> textContents = [];
        if (!string.IsNullOrEmpty(responseBody?.Completion))
        {
            textContents.Add(new TextContent(responseBody.Completion));
        }

        return textContents;
    }

    /// <inheritdoc/>
    public ConverseRequest GetConverseRequest(string modelId, ChatHistory chatHistory, PromptExecutionSettings? settings)
    {
        var messages = BedrockModelUtilities.BuildMessageList(chatHistory);
        var systemMessages = BedrockModelUtilities.GetSystemMessages(chatHistory);

        var exec = AmazonClaudePromptExecutionSettings.FromExecutionSettings(settings);
        var temp = BedrockModelUtilities.GetExtensionDataValue<float?>(settings?.ExtensionData, "temperature") ?? exec.Temperature;
        var topP = BedrockModelUtilities.GetExtensionDataValue<float?>(settings?.ExtensionData, "top_p") ?? exec.TopP;
        var maxTokens = BedrockModelUtilities.GetExtensionDataValue<int?>(settings?.ExtensionData, "max_tokens_to_sample") ?? exec.MaxTokensToSample;
        var stopSequences = BedrockModelUtilities.GetExtensionDataValue<List<string>>(settings?.ExtensionData, "stop_sequences") ?? exec.StopSequences;

        var inferenceConfig = new InferenceConfiguration();
        BedrockModelUtilities.SetPropertyIfNotNull(() => temp, value => inferenceConfig.Temperature = value);
        BedrockModelUtilities.SetPropertyIfNotNull(() => topP, value => inferenceConfig.TopP = value);
        inferenceConfig.MaxTokens = maxTokens; // Max Token Count required (cannot be null).
        BedrockModelUtilities.SetStopSequenceIfNotNull(() => stopSequences, value => inferenceConfig.StopSequences = value);

        var additionalModelRequestFields = new Document();
        ToolConfiguration? toolConfig = null;

        if (settings?.ExtensionData != null)
        {
            if (settings.ExtensionData.ContainsKey("tools"))
            {
                toolConfig = new ToolConfiguration
                {
                    Tools = BedrockModelUtilities.GetExtensionDataValue<List<Tool>>(settings.ExtensionData, "tools"),
                    ToolChoice = BedrockModelUtilities.GetExtensionDataValue<ToolChoice?>(settings.ExtensionData, "tool_choice") // Optional
                };
            }
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
            ToolConfig = toolConfig
        };

        return converseRequest;
    }

    /// <inheritdoc/>
    public IEnumerable<string> GetTextStreamOutput(JsonNode chunk)
    {
        var text = chunk["completion"]?.ToString();
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

        var exec = AmazonClaudePromptExecutionSettings.FromExecutionSettings(settings);
        var temp = BedrockModelUtilities.GetExtensionDataValue<float?>(settings?.ExtensionData, "temperature") ?? exec.Temperature;
        var topP = BedrockModelUtilities.GetExtensionDataValue<float?>(settings?.ExtensionData, "top_p") ?? exec.TopP;
        var maxTokens = BedrockModelUtilities.GetExtensionDataValue<int?>(settings?.ExtensionData, "max_tokens_to_sample") ?? exec.MaxTokensToSample;
        var stopSequences = BedrockModelUtilities.GetExtensionDataValue<List<string>>(settings?.ExtensionData, "stop_sequences") ?? exec.StopSequences;

        var inferenceConfig = new InferenceConfiguration();
        BedrockModelUtilities.SetPropertyIfNotNull(() => temp, value => inferenceConfig.Temperature = value);
        BedrockModelUtilities.SetPropertyIfNotNull(() => topP, value => inferenceConfig.TopP = value);
        inferenceConfig.MaxTokens = maxTokens; // Max Token Count required (cannot be null).
        BedrockModelUtilities.SetStopSequenceIfNotNull(() => stopSequences, value => inferenceConfig.StopSequences = value);

        var additionalModelRequestFields = new Document();
        ToolConfiguration? toolConfig = null;

        if (settings?.ExtensionData != null)
        {
            if (settings.ExtensionData.ContainsKey("tools"))
            {
                toolConfig = new ToolConfiguration
                {
                    Tools = BedrockModelUtilities.GetExtensionDataValue<List<Tool>>(settings.ExtensionData, "tools"),
                    ToolChoice = BedrockModelUtilities.GetExtensionDataValue<ToolChoice?>(settings.ExtensionData, "tool_choice") // Optional
                };
            }
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
            ToolConfig = toolConfig
        };

        return converseRequest;
    }
}
