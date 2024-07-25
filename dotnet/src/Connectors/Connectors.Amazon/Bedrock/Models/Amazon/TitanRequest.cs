﻿// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;
using Amazon.BedrockRuntime.Model;
using Amazon.Runtime.Documents;
using Connectors.Amazon.Core.Requests;

namespace Connectors.Amazon.Models.Amazon;

/// <summary>
/// The Amazon Titan Request objects.
/// </summary>
public static class TitanRequest
{
    /// <summary>
    /// The Amazon Titan Text Generation Request object.
    /// </summary>
    [Serializable]
    public sealed class TitanTextGenerationRequest : ITextGenerationRequest
    {
        /// <summary>
        /// The provided input text string for text generation response.
        /// </summary>
        [JsonPropertyName("inputText")]
        public required string InputText { get; set; }
        /// <summary>
        /// Text generation configurations as required by Amazon Titan request body.
        /// </summary>
        [JsonPropertyName("textGenerationConfig")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AmazonTitanTextGenerationConfig? TextGenerationConfig { get; set; }

        int? ITextGenerationRequest.MaxTokens => this.TextGenerationConfig?.MaxTokenCount;

        double? ITextGenerationRequest.TopP => this.TextGenerationConfig?.TopP;

        double? ITextGenerationRequest.Temperature => this.TextGenerationConfig?.Temperature;

        IList<string>? ITextGenerationRequest.StopSequences => this.TextGenerationConfig?.StopSequences;
    }
    /// <summary>
    /// Amazon Titan Text Generation Configurations.
    /// </summary>
    [Serializable]
    public class AmazonTitanTextGenerationConfig
    {
        /// <summary>
        /// Top P controls token choices, based on the probability of the potential choices. The range is 0 to 1. The default is 1.
        /// </summary>
        [JsonPropertyName("topP")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public float TopP { get; set; }

        /// <summary>
        /// The Temperature value ranges from 0 to 1, with 0 being the most deterministic and 1 being the most creative.
        /// </summary>
        [JsonPropertyName("temperature")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public float Temperature { get; set; }

        /// <summary>
        /// Configures the maximum number of tokens in the generated response. The range is 0 to 4096. The default is 512.
        /// </summary>
        [JsonPropertyName("maxTokenCount")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MaxTokenCount { get; set; }

        /// <summary>
        /// Use | (pipe) characters (maximum 20 characters).
        /// </summary>
        [JsonPropertyName("stopSequences")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IList<string>? StopSequences { get; set; } = new List<string>();
    }
    /// <summary>
    /// Amazon Titan Chat Completion Parameters for the Converse Request object.
    /// </summary>
    public class TitanChatCompletionRequest : IChatCompletionRequest
    {
        /// <inheritdoc />
        public List<Message>? Messages { get; set; }
        /// <inheritdoc />
        public List<SystemContentBlock>? System { get; set; }
        /// <inheritdoc />
        public InferenceConfiguration? InferenceConfig { get; set; }
        /// <inheritdoc />
        public Document AdditionalModelRequestFields { get; set; }
        /// <inheritdoc />
        public List<string>? AdditionalModelResponseFieldPaths { get; set; }
        /// <inheritdoc />
        public GuardrailConfiguration? GuardrailConfig { get; set; }
        /// <inheritdoc />
        public string? ModelId { get; set; }
        /// <inheritdoc />
        public ToolConfiguration? ToolConfig { get; set; }
    }
    /// <summary>
    /// Request object for text embedding generation for Amazon Titan.
    /// </summary>
    public class TitanTextEmbeddingRequest : ITextEmbeddingRequest
    {
        [JsonPropertyName("inputText")]
        public string InputText { get; set; }

        [JsonPropertyName("dimensions")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Dimensions { get; set; }

        [JsonPropertyName("normalize")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Normalize { get; set; }
    }
}
