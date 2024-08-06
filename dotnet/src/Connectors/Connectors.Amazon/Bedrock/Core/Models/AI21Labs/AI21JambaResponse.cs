﻿// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

namespace Connectors.Amazon.Core;

/// <summary>
/// AI21JambaResponse objects for Bedrock Runtime actions.
/// </summary>
internal static class AI21JambaResponse
{
    /// <summary>
    /// AI21 Text Generation Response object (from Invoke).
    /// </summary>
    [Serializable]
    internal class AI21TextResponse
    {
        /// <summary>
        /// A unique ID for the request (not the message). Repeated identical requests get different IDs. However, for a streaming response, the ID will be the same for all responses in the stream.
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        /// <summary>
        /// One or more responses, depending on the n parameter from the request.
        /// </summary>
        [JsonPropertyName("choices")]
        public List<Choice>? Choices { get; set; }
        /// <summary>
        /// The token counts for this request. Per-token billing is based on the prompt token and completion token counts and rates.
        /// </summary>
        [JsonPropertyName("usage")]
        public Usage? Use { get; set; }
        /// <summary>
        /// The members for the Choice class as required by AI21 Labs Jamba.
        /// </summary>
        [Serializable]
        internal class Choice
        {
            /// <summary>
            /// Zero-based index of the message in the list of messages. Note that this might not correspond with the position in the response list.
            /// </summary>
            [JsonPropertyName("index")]
            public int Index { get; set; }
            /// <summary>
            /// The message generated by the model. Same structure as the request message, with role and content members.
            /// </summary>
            [JsonPropertyName("message")]
            public Message? Message { get; set; }
            /// <summary>
            /// Why the message ended. Possible reasons:
            /// stop: The response ended naturally as a complete answer(due to end-of-sequence token) or because the model generated a stop sequence provided in the request.
            /// length: The response ended by reaching max_tokens.
            /// </summary>
            [JsonPropertyName("finish_reason")]
            public string? FinishReason { get; set; }
        }
        /// <summary>
        /// Message object for the model with role and content as required.
        /// </summary>
        [Serializable]
        internal class Message
        {
            /// <summary>
            /// The role of the message author. One of the following values:
            /// user: Input provided by the user.Any instructions given here that conflict with instructions given in the system prompt take precedence over the system prompt instructions.
            /// assistant: Response generated by the model.
            /// system: Initial instructions provided to the system to provide general guidance on the tone and voice of the generated message.An initial system message is optional but recommended to provide guidance on the tone of the chat.For example, "You are a helpful chatbot with a background in earth sciences and a charming French accent."
            /// </summary>
            [JsonPropertyName("role")]
            public string? Role { get; set; }
            /// <summary>
            /// The content of the message.
            /// </summary>
            [JsonPropertyName("content")]
            public string? Content { get; set; }
        }
        /// <summary>
        /// The token counts for this request. Per-token billing is based on the prompt token and completion token counts and rates.
        /// </summary>
        [Serializable]
        internal class Usage
        {
            /// <summary>
            /// Number of tokens in the prompt for this request. Note that the prompt token includes the entire message history, plus extra tokens needed by the system when combining the list of prompt messages into a single message, as required by the model. The number of extra tokens is typically proportional to the number of messages in the thread, and should be relatively small.
            /// </summary>
            [JsonPropertyName("prompt_tokens")]
            public int PromptTokens { get; set; }
            /// <summary>
            /// Number of tokens in the response message.
            /// </summary>
            [JsonPropertyName("completion_tokens")]
            public int CompletionTokens { get; set; }
            /// <summary>
            /// prompt_tokens + completion_tokens.
            /// </summary>
            [JsonPropertyName("total_tokens")]
            public int TotalTokens { get; set; }
        }
    }
}
