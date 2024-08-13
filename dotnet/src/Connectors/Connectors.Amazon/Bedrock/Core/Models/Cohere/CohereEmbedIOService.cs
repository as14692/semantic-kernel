// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json;
using Amazon.BedrockRuntime.Model;

namespace Microsoft.SemanticKernel.Connectors.Amazon.Core;

/// <summary>
/// Embedding input-output service for Cohere. Cohere-embed is not a Command or Command R model so is differentiated as a service.
/// </summary>
public class CohereEmbedIOService : IBedrockTextEmbeddingIOService
{
    /// <summary>
    /// Builds the InvokeModelRequest body for text embedding generation requests.
    /// This model does not support text embedding generation currently.
    /// </summary>
    /// <param name="data">The data to be passed into the request.</param>
    /// <param name="modelId">The model for the request.</param>
    /// <returns></returns>
    public object GetEmbeddingRequestBody(string data, string modelId)
    {
        // Until Semantic Kernel provides execution settings parameter to pass into GenerateEmbeddingsAsync, these parameter cannot be altered from default.
        return new
        {
            texts = new List<string> { data },
            input_type = "search_document",
            truncate = "END",
            embedding_types = new List<string>()
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
            var responseBody = JsonSerializer.Deserialize<CohereEmbedResponse>(reader.ReadToEnd());
            if (responseBody?.Embeddings is { Count: > 0 } embeddings)
            {
                var firstEmbedding = embeddings[0];
                return new ReadOnlyMemory<float>(firstEmbedding.ToArray());
            }

            return ReadOnlyMemory<float>.Empty;
        }
    }
}
