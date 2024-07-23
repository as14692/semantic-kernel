// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json;
using System.Text.Json.Nodes;
using Amazon.BedrockRuntime.Model;
using Connectors.Amazon.Core.Requests;
using Connectors.Amazon.Core.Responses;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Connectors.Amazon.Models.Cohere;

/// <summary>
/// Embedding input-output service for Cohere. Cohere-embed is not a Command or Command R model so is differentiated as a service.
/// </summary>
public class CohereEmbedIOService : IBedrockModelIOService<ITextEmbeddingRequest, ITextEmbeddingResponse>
{
    /// <summary>
    /// This class is just for embedding.
    /// </summary>
    /// <param name="prompt">The input prompt for text generation.</param>
    /// <param name="executionSettings">Optional prompt execution settings.</param>
    /// <returns></returns>
    public object GetInvokeModelRequestBody(string prompt, PromptExecutionSettings? executionSettings = null)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// This class is just for embedding.
    /// </summary>
    /// <param name="response">The InvokeModelResponse object provided by the Bedrock InvokeModelAsync output.</param>
    /// <returns></returns>
    public IReadOnlyList<TextContent> GetInvokeResponseBody(InvokeModelResponse response)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// This class is just for embedding.
    /// </summary>
    /// <param name="chunk"></param>
    /// <returns></returns>
    public IEnumerable<string> GetTextStreamOutput(JsonNode chunk)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }
    /// <summary>
    /// This class is just for embedding.
    /// </summary>
    /// <param name="modelId"></param>
    /// <param name="chatHistory"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public ConverseStreamRequest GetConverseStreamRequest(string modelId, ChatHistory chatHistory, PromptExecutionSettings settings)
    {
        throw new NotImplementedException();
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
        return new CohereEmbedRequest()
        {
            Texts = [data],
            InputType = "search_document", //until Semantic Kernel provides parameter to pass into GenerateEmbeddingsAsync, this parameter cannot be altered.
            Truncate = "NONE",
            EmbeddingTypes = []
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
        using (var memoryStream = new MemoryStream())
        {
            response.Body.CopyToAsync(memoryStream).ConfigureAwait(false).GetAwaiter().GetResult();
            memoryStream.Position = 0;
            using (var reader = new StreamReader(memoryStream))
            {
                var responseBody = JsonSerializer.Deserialize<CohereEmbedResponse>(reader.ReadToEnd());
                if (responseBody?.Embeddings != null && responseBody.Embeddings.Count > 0)
                {
                    var firstEmbedding = responseBody.Embeddings[0];
                    return new ReadOnlyMemory<float>(firstEmbedding.ToArray());
                }
                return new ReadOnlyMemory<float>();
            }
        }
    }
}
