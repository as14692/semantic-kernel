// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;
using Connectors.Amazon.Core.Requests;

namespace Connectors.Amazon.Models.Cohere;

/// <summary>
/// Text Embedding Generation request body for Cohere.
/// </summary>
public class CohereEmbedRequest : ITextEmbeddingRequest
{
    /// <summary>
    /// An array of strings for the model to embed. For optimal performance, we recommend reducing the length of each text to less than 512 tokens. 1 token is about 4 characters.
    /// </summary>
    [JsonPropertyName("texts")]
    public IReadOnlyList<string>? Texts { get; set; }

    /// <summary>
    /// Prepends special tokens to differentiate each type from one another. You should not mix different types together, except when mixing type for search and retrieval. In this case, embed your corpus with the search_document type and embedded queries with type search_query type.
    /// search_document – In search use-cases, use search_document when you encode documents for embeddings that you store in a vector database.
    /// search_query – Use search_query when querying your vector DB to find relevant documents.
    /// classification – Use classification when using embeddings as an input to a text classifier.
    /// clustering – Use clustering to cluster the embeddings.
    /// </summary>
    [JsonPropertyName("input_type")]
    public string? InputType { get; set; }

    /// <summary>
    /// Specifies how to truncate the input text if it exceeds the maximum length.
    /// </summary>
    [JsonPropertyName("truncate")]
    public string? Truncate { get; set; }

    /// <summary>
    /// Specifies the types of embeddings you want to have returned. Optional and default is None, which returns the Embed Floats response type. Can be one or more of the following types:
    /// float – Use this value to return the default float embeddings.
    /// int8 – Use this value to return signed int8 embeddings.
    /// uint8 – Use this value to return unsigned int8 embeddings.
    /// binary – Use this value to return signed binary embeddings.
    /// ubinary – Use this value to return unsigned binary embeddings.
    /// </summary>
    [JsonPropertyName("embedding_types")]
    public IReadOnlyList<string>? EmbeddingTypes { get; set; }

    /// <summary>
    /// The prompt given to the Bedrock model.
    /// </summary>
    [JsonIgnore]
    public string InputText
    {
        get
        {
            if (this.Texts != null && this.Texts.Count > 0)
            {
                return string.Join(", ", this.Texts);
            }
            return string.Empty;
        }
    }
}
