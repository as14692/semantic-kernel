// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.BedrockRuntime;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Amazon.Core;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Services;

namespace Connectors.Amazon.Bedrock.Services;

/// <summary>
/// Bedrock Text Embedding Generation Service.
/// </summary>
public class BedrockTextEmbeddingGenerationService : ITextEmbeddingGenerationService
{
    private readonly Dictionary<string, object?> _attributesInternal = [];
    private readonly BedrockTextEmbeddingClient _textEmbeddingClient;

    /// <inheritdoc />
    public IReadOnlyDictionary<string, object?> Attributes => this._attributesInternal;

    /// <summary>
    /// Initializes an instance of the BedrockTextEmbeddingGenerationService using an IAmazonBedrockRuntime object passed in by the user.
    /// </summary>
    /// <param name="modelId"></param>
    /// <param name="bedrockApi"></param>
    /// <param name="loggerFactory"></param>
    public BedrockTextEmbeddingGenerationService(string modelId, IAmazonBedrockRuntime bedrockApi, ILoggerFactory? loggerFactory = null)
    {
        this._textEmbeddingClient = new BedrockTextEmbeddingClient(modelId, bedrockApi, loggerFactory);
        this._attributesInternal.Add(AIServiceExtensions.ModelIdKey, modelId);
    }

    /// <inheritdoc/>
    public Task<IList<ReadOnlyMemory<float>>> GenerateEmbeddingsAsync(
        IList<string> data,
        Kernel? kernel = null,
        CancellationToken cancellationToken = default)
    {
        return this._textEmbeddingClient.GetEmbeddingsAsync(data, kernel, cancellationToken);
    }
}
