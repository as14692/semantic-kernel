﻿// Copyright (c) Microsoft. All rights reserved.

using Amazon.BedrockRuntime.Model;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Microsoft.SemanticKernel.Connectors.Amazon.Core;

internal interface IBedrockChatCompletionIOService
{
    /// <summary>
    /// Builds the converse request given the chat history and model ID passed in by the user. This request is to be passed into the Bedrock Converse API call.
    /// </summary>
    /// <param name="modelId">The model ID to be used as a request parameter.</param>
    /// <param name="chatHistory">The messages for the converse call.</param>
    /// <param name="settings">Optional prompt execution settings/</param>
    /// <returns>The ConverseRequest object for the converse call.</returns>
    internal ConverseRequest GetConverseRequest(string modelId, ChatHistory chatHistory, PromptExecutionSettings? settings = null);

    /// <summary>
    /// Builds the converse stream request given the chat history and model ID passed in by the user. This request is to be passed into the Bedrock Converse API call.
    /// </summary>
    /// <param name="modelId">The model ID for the request.</param>
    /// <param name="chatHistory">The ChatHistory object to be converted to messages for the stream converse request.</param>
    /// <param name="settings">Optional PromptExecutionSettings for the request.</param>
    /// <returns>The ConverseStreamRequest for the converse stream call.</returns>
    internal ConverseStreamRequest GetConverseStreamRequest(string modelId, ChatHistory chatHistory, PromptExecutionSettings? settings = null);
}
