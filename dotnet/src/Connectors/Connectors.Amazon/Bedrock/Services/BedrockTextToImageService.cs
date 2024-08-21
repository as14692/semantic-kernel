// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.BedrockRuntime;
using Connectors.Amazon.Bedrock.Core.Clients;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Services;
using Microsoft.SemanticKernel.TextToImage;

namespace Connectors.Amazon.Bedrock.Services;

/// <summary>
/// Service for Text to Image generation with Bedrock.
/// </summary>
public class BedrockTextToImageService : ITextToImageService
{
    private readonly Dictionary<string, object?> _attributesInternal = [];
    private readonly BedrockTextToImageClient _textToImageClient;

    /// <inheritdoc />
    public IReadOnlyDictionary<string, object?> Attributes => this._attributesInternal;

    /// <summary>
    /// Initialize client object for text to image service.
    /// </summary>
    /// <param name="modelId"></param>
    /// <param name="bedrockApi"></param>
    /// <param name="loggerFactory"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public BedrockTextToImageService(string modelId, IAmazonBedrockRuntime bedrockApi, ILoggerFactory? loggerFactory = null)
    {
        this._textToImageClient = new BedrockTextToImageClient(modelId, bedrockApi, loggerFactory);
        this._attributesInternal.Add(AIServiceExtensions.ModelIdKey, modelId);
    }
    /// <inheritdoc />
    /// <returns></returns>
    public Task<string> GenerateImageAsync(
        string description,
        int width,
        int height,
        Kernel? kernel = null,
        CancellationToken cancellationToken = default)
        => this._textToImageClient.GetImageAsync(description, width, height, kernel, cancellationToken);
}
