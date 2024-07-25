// Copyright (c) Microsoft. All rights reserved.

using Amazon.BedrockRuntime;
using Connectors.Amazon.Bedrock.Core.Clients;
using Connectors.Amazon.Core.Requests;
using Connectors.Amazon.Core.Responses;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Services;
using Microsoft.SemanticKernel.TextToImage;

namespace Connectors.Amazon.Bedrock.Services;

public class BedrockTextToImageService : BedrockTextToImageClient<ITextToImageRequest, ITextToImageResponse>, ITextToImageService
{
    private readonly Dictionary<string, object?> _attributesInternal = [];

    /// <inheritdoc />
    public IReadOnlyDictionary<string, object?> Attributes => this._attributesInternal;

    public BedrockTextToImageService(string modelId, IAmazonBedrockRuntime bedrockApi)
        : base(modelId, bedrockApi)
    {
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
        => this.GetImageAsync(description, width, height, kernel, cancellationToken);
}
