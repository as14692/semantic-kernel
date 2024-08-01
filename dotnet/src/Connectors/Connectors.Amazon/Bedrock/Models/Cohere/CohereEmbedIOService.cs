// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json;
using System.Text.Json.Nodes;
using Amazon.BedrockRuntime.Model;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Connectors.Amazon.Models.Cohere;

/// <summary>
/// Embedding input-output service for Cohere. Cohere-embed is not a Command or Command R model so is differentiated as a service.
/// </summary>
public class CohereEmbedIOService : IBedrockModelIOService
{
    /// <summary>
    /// This class is just for embedding.
    /// </summary>
    /// <param name="modelId">The model.</param>
    /// <param name="prompt">The input prompt for text generation.</param>
    /// <param name="executionSettings">Optional prompt execution settings.</param>
    /// <returns></returns>
    public object GetInvokeModelRequestBody(string modelId, string prompt, PromptExecutionSettings? executionSettings = null)
    {
        throw new NotImplementedException("This model is just for text embedding.");
    }
    /// <summary>
    /// This class is just for embedding.
    /// </summary>
    /// <param name="response">The InvokeModelResponse object provided by the Bedrock InvokeModelAsync output.</param>
    /// <returns></returns>
    public IReadOnlyList<TextContent> GetInvokeResponseBody(InvokeModelResponse response)
    {
        throw new NotImplementedException("This model is just for text embedding.");
    }

    /// <summary>
    /// This class is just for embedding.
    /// </summary>
    /// <param name="chunk"></param>
    /// <returns></returns>
    public IEnumerable<string> GetTextStreamOutput(JsonNode chunk)
    {
        throw new NotImplementedException("This model is just for text embedding.");
    }

    /// <summary>
    /// This class is just for embedding.
    /// </summary>
    /// <param name="modelId"></param>
    /// <param name="chatHistory"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public ConverseRequest GetConverseRequest(string modelId, ChatHistory chatHistory, PromptExecutionSettings? settings = null)
    {
        throw new NotImplementedException("This model is just for text embedding.");
    }
    /// <summary>
    /// This class is just for embedding.
    /// </summary>
    /// <param name="modelId"></param>
    /// <param name="chatHistory"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public ConverseStreamRequest GetConverseStreamRequest(string modelId, ChatHistory chatHistory, PromptExecutionSettings? settings = null)
    {
        throw new NotImplementedException("This model is just for text embedding.");
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
        using var memoryStream = new MemoryStream();
        response.Body.CopyToAsync(memoryStream).ConfigureAwait(false).GetAwaiter().GetResult();
        memoryStream.Position = 0;
        using var reader = new StreamReader(memoryStream);
        var responseBody = JsonSerializer.Deserialize<CohereEmbedResponse>(reader.ReadToEnd());
        if (responseBody?.Embeddings is not { Count: > 0 })
        {
            return new ReadOnlyMemory<float>();
        }
        var firstEmbedding = responseBody.Embeddings[0];
        return new ReadOnlyMemory<float>(firstEmbedding.ToArray());
    }

    /// <inheritdoc />
    /// Not supported by this model.
    public object GetInvokeRequestBodyForTextToImage(
        string modelId,
        string description,
        int width,
        int height,
        PromptExecutionSettings? executionSettings = null)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    /// Not supported by this model.
    public string GetInvokeResponseForImage(InvokeModelResponse response)
    {
        throw new NotImplementedException();
    }
}
