// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Connectors.Amazon.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.SemanticKernel;

namespace Connectors.Amazon.Bedrock.Core.Clients;

/// <summary>
/// Client for Text to image service.
/// </summary>
internal sealed class BedrockTextToImageClient
{
    private readonly string _modelId;
    private readonly IAmazonBedrockRuntime _bedrockApi;
    private readonly IBedrockModelIOService _ioService;
    private readonly ILogger _logger;

    public BedrockTextToImageClient(string modelId, IAmazonBedrockRuntime bedrockApi, ILoggerFactory? loggerFactory = null)
    {
        this._modelId = modelId;
        this._bedrockApi = bedrockApi;
        this._ioService = new BedrockClientIOService().GetIOService(modelId);
        this._logger = loggerFactory?.CreateLogger(this.GetType()) ?? NullLogger.Instance;
    }

    internal async Task<string> GetImageAsync(
        string description,
        int width,
        int height,
        Kernel? kernel = null,
        CancellationToken cancellationToken = default)
    {
        var requestBody = this._ioService.GetInvokeRequestBodyForTextToImage(this._modelId, description, width, height);
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
            response = await this._bedrockApi.InvokeModelAsync(invokeRequest, cancellationToken).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error while invoking model {ModelId}", this._modelId);
            throw;
        }
        return this._ioService.GetInvokeResponseForImage(response);
    }
}
