﻿// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Connectors.Amazon.Core.Requests;
using Connectors.Amazon.Core.Responses;
using Connectors.Amazon.Models;
using Connectors.Amazon.Models.Amazon;
using Connectors.Amazon.Models.Cohere;

namespace Microsoft.SemanticKernel.Connectors.Amazon.Core;

/// <summary>
/// Represents a client for interacting with text embedding through Bedrock.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class BedrockTextEmbeddingClient<TRequest, TResponse>
    where TRequest : ITextEmbeddingRequest
    where TResponse : ITextEmbeddingResponse
{
    private readonly string _modelId;
    private readonly IAmazonBedrockRuntime _bedrockApi;
    private readonly IBedrockModelIOService<ITextEmbeddingRequest, ITextEmbeddingResponse> _ioService;
    /// <summary>
    /// Builds the client object and registers the model input-output service given the user's passed in model ID.
    /// </summary>
    /// <param name="modelId"></param>
    /// <param name="bedrockApi"></param>
    /// <exception cref="ArgumentException"></exception>
    protected BedrockTextEmbeddingClient(string modelId, IAmazonBedrockRuntime bedrockApi)
    {
        this._modelId = modelId;
        this._bedrockApi = bedrockApi;
        string[] parts = modelId.Split('.'); //modelId looks like "amazon.titan-embed-text-v1:0"
        string modelProvider = parts[0];
        string modelName = parts.Length > 1 ? parts[1] : string.Empty;
        switch (modelProvider)
        {
            case "amazon":
                if (modelName.StartsWith("titan-", StringComparison.OrdinalIgnoreCase))
                {
                    this._ioService = new AmazonIOService();
                    break;
                }
                throw new ArgumentException($"Unsupported Amazon model: {modelId}");
            case "cohere":
                if (modelName.StartsWith("embed-", StringComparison.OrdinalIgnoreCase))
                {
                    this._ioService = new CohereEmbedIOService();
                    break;
                }
                throw new ArgumentException($"Unsupported Cohere model: {modelId}");
            default:
                throw new ArgumentException($"Unsupported model provider: {modelProvider}");
        }
    }

    /// <summary>
    /// Generates the text embeddings.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="kernel"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<IList<ReadOnlyMemory<float>>> GetEmbeddingsAsync(
        IList<string> data,
        Kernel? kernel = null,
        CancellationToken cancellationToken = default)
    {
        var finalList = new List<ReadOnlyMemory<float>>();
        foreach (var stringInput in data)
        {
            var requestBody = this._ioService.GetEmbeddingRequestBody(stringInput, this._modelId);
            var invokeRequest = new InvokeModelRequest
            {
                ModelId = this._modelId,
                Accept = "*/*",
                ContentType = "application/json",
                Body = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(requestBody))
            };
            var response = await this._bedrockApi.InvokeModelAsync(invokeRequest, cancellationToken).ConfigureAwait(true);
            var output = this._ioService.GetEmbeddingResponseBody(response);
            finalList.Add(output);
        }
        return finalList;
    }
}
