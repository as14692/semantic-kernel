// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

namespace Microsoft.SemanticKernel.Connectors.Amazon.Core;

/// <summary>
/// Text Embedding Generation response body for Cohere embed models.
/// </summary>
public class CohereEmbedResponse
{
    /// <summary>
    /// An array of embeddings, where each embedding is an array of floats with 1024 elements. The length of the embeddings array will be the same as the length of the original texts array.
    /// </summary>
    [JsonPropertyName("embeddings")]
    public List<List<float>>? Embeddings { get; set; }
    /// <summary>
    /// An identifier for the response.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    /// <summary>
    /// The response type. This value is always embeddings_floats.
    /// </summary>
    [JsonPropertyName("response_type")]
    public string? ResponseType { get; set; }
    /// <summary>
    /// An array containing the text entries for which embeddings were returned.
    /// </summary>
    [JsonPropertyName("texts")]
    public List<string>? Texts { get; set; }
}
