// Copyright (c) Microsoft. All rights reserved.

using Amazon.BedrockRuntime;
using Connectors.Amazon.Core.Requests;
using Connectors.Amazon.Core.Responses;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Amazon.Core;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Services;

namespace Connectors.Amazon.Bedrock.Services;

/// <summary>
/// Bedrock Text Embedding Generation Service.
/// </summary>
public class BedrockTextEmbeddingGenerationService : BedrockTextEmbeddingClient<ITextEmbeddingRequest, ITextEmbeddingResponse>, ITextEmbeddingGenerationService
{
    private readonly Dictionary<string, object?> _attributesInternal = [];

    /// <inheritdoc />
    public IReadOnlyDictionary<string, object?> Attributes => this._attributesInternal;

    /// <summary>
    /// Initializes an instance of the BedrockTextEmbeddingGenerationService using an IAmazonBedrockRuntime object passed in by the user.
    /// </summary>
    /// <param name="modelId"></param>
    /// <param name="bedrockApi"></param>
    public BedrockTextEmbeddingGenerationService(string modelId, IAmazonBedrockRuntime bedrockApi)
        : base(modelId, bedrockApi)
    {
        this._attributesInternal.Add(AIServiceExtensions.ModelIdKey, modelId);
    }

    /// <inheritdoc/>
    public Task<IList<ReadOnlyMemory<float>>> GenerateEmbeddingsAsync(
        IList<string> data,
        Kernel? kernel = null,
        CancellationToken cancellationToken = default)
    {
        return this.GetEmbeddingsAsync(data, kernel, cancellationToken);
    }
}
