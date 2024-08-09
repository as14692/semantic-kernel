// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.SemanticKernel.Connectors.Amazon.Core;

/// <summary>
/// Represents a client for interacting with text embedding through Bedrock.
/// </summary>
internal sealed class BedrockTextEmbeddingClient
{
    private readonly string _modelId;
    private readonly IAmazonBedrockRuntime _bedrockApi;
    private readonly IBedrockModelIOService _ioService;
    private readonly ILogger _logger;
    /// <summary>
    /// Builds the client object and registers the model input-output service given the user's passed in model ID.
    /// </summary>
    /// <param name="modelId"></param>
    /// <param name="bedrockApi"></param>
    /// <param name="loggerFactory"></param>
    /// <exception cref="ArgumentException"></exception>
    public BedrockTextEmbeddingClient(string modelId, IAmazonBedrockRuntime bedrockApi, ILoggerFactory? loggerFactory = null)
    {
        this._modelId = modelId;
        this._bedrockApi = bedrockApi;
        var clientService = new BedrockClientIOService();
        this._ioService = clientService.GetIOService(modelId);
        this._logger = loggerFactory?.CreateLogger(this.GetType()) ?? NullLogger.Instance;
    }

    /// <summary>
    /// Generates the text embeddings.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="kernel"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    internal async Task<IList<ReadOnlyMemory<float>>> GetEmbeddingsAsync(
        IList<string> data,
        Kernel? kernel = null,
        CancellationToken cancellationToken = default)
    {
        Verify.NotNullOrEmpty(data);
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
            InvokeModelResponse? response;
            try
            {
                response = await this._bedrockApi.InvokeModelAsync(invokeRequest, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Can't invoke '{ModelId}'. Reason: {Error}", this._modelId, ex.Message);
                throw;
            }
            var output = this._ioService.GetEmbeddingResponseBody(response);
            finalList.Add(output);
        }
        return finalList;
    }
}
