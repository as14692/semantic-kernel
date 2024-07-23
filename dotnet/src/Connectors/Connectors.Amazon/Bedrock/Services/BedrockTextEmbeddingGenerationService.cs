// Copyright (c) Microsoft. All rights reserved.

using Amazon;
using Amazon.BedrockRuntime;
using Amazon.Runtime;
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
    /// <summary>
    /// Initializes an instance of the BedrockTextEmbeddingGenerationService by creating a new AmazonBedrockRuntimeClient().
    /// </summary>
    /// <param name="modelId"></param>
    public BedrockTextEmbeddingGenerationService(string modelId)
        : base(modelId, new AmazonBedrockRuntimeClient())
    {
        this._attributesInternal.Add(AIServiceExtensions.ModelIdKey, modelId);
    }
    /// <summary>
    /// Initializes an instance of the BedrockTextEmbeddingGenerationService.
    /// Constructs AmazonBedrockRuntimeClient with the credentials loaded from the application's
    /// default configuration, and if unsuccessful from the Instance Profile service on an EC2 instance.
    /// </summary>
    /// <param name="modelId">The model to be used for chat completion.</param>
    /// <param name="region">The region to connect.</param>
    public BedrockTextEmbeddingGenerationService(string modelId, RegionEndpoint region)
        : base(modelId, new AmazonBedrockRuntimeClient(region))
    {
        this._attributesInternal.Add(AIServiceExtensions.ModelIdKey, modelId);
    }
    /// <summary>
    /// Initializes an instance of the BedrockTextEmbeddingGenerationService.
    /// Constructs AmazonBedrockRuntimeClient with the credentials loaded from the application's
    /// default configuration, and if unsuccessful from the Instance Profile service on an EC2 instance.
    /// </summary>
    /// <param name="modelId">The model to be used for chat completion.</param>
    /// <param name="clientConfig">The AmazonBedrockRuntimeClient Configuration Object.</param>
    public BedrockTextEmbeddingGenerationService(string modelId, AmazonBedrockRuntimeConfig clientConfig)
        : base(modelId, new AmazonBedrockRuntimeClient(clientConfig))
    {
        this._attributesInternal.Add(AIServiceExtensions.ModelIdKey, modelId);
    }
    /// <summary>
    /// Initializes an instance of the BedrockTextEmbeddingGenerationService with AWSCredentials object for authentication.
    /// </summary>
    /// <param name="modelId">The model to be used for chat completion. </param>
    /// <param name="awsCredentials">AWS Credentials. </param>
    public BedrockTextEmbeddingGenerationService(string modelId, AWSCredentials awsCredentials)
        : base(modelId, new AmazonBedrockRuntimeClient(awsCredentials))
    {
        this._attributesInternal.Add(AIServiceExtensions.ModelIdKey, modelId);
    }
    /// <summary>
    /// Initializes an instance of the BedrockTextEmbeddingGenerationService with AWSCredentials object for authentication.
    /// </summary>
    /// <param name="modelId">The model to be used for chat completion.</param>
    /// <param name="awsCredentials">AWS Credentials.</param>
    /// <param name="region">The region to connect.</param>
    public BedrockTextEmbeddingGenerationService(string modelId, AWSCredentials awsCredentials, RegionEndpoint region)
        : base(modelId, new AmazonBedrockRuntimeClient(awsCredentials, region))
    {
        this._attributesInternal.Add(AIServiceExtensions.ModelIdKey, modelId);
    }
    /// <summary>
    /// Initializes an instance of the BedrockTextEmbeddingGenerationService.
    /// Constructs AmazonBedrockRuntimeClient with AWS Credentials and an
    /// AmazonBedrockRuntimeClient Configuration object.
    /// </summary>
    /// <param name="modelId">The model to be used for chat completion.</param>
    /// <param name="awsCredentials">AWS Credentials.</param>
    /// <param name="clientConfig">The AmazonBedrockRuntimeClient Configuration Object</param>
    public BedrockTextEmbeddingGenerationService(string modelId, AWSCredentials awsCredentials, AmazonBedrockRuntimeConfig clientConfig)
        : base(modelId, new AmazonBedrockRuntimeClient(awsCredentials, clientConfig))
    {
        this._attributesInternal.Add(AIServiceExtensions.ModelIdKey, modelId);
    }
    /// <summary>
    /// Initializes an instance of the BedrockTextEmbeddingGenerationService.
    /// Constructs AmazonBedrockRuntimeClient with AWS Access Key ID and AWS Secret Key
    /// </summary>
    /// <param name="modelId">The model to be used for chat completion.</param>
    /// <param name="awsAccessKeyId">AWS Access Key ID</param>
    /// <param name="awsSecretAccessKey">AWS Secret Access Key</param>
    public BedrockTextEmbeddingGenerationService(string modelId, string awsAccessKeyId, string awsSecretAccessKey)
        : base(modelId, new AmazonBedrockRuntimeClient(awsAccessKeyId, awsSecretAccessKey))
    {
        this._attributesInternal.Add(AIServiceExtensions.ModelIdKey, modelId);
    }
    /// <summary>
    /// Initializes an instance of the BedrockTextEmbeddingGenerationService.
    /// Constructs AmazonBedrockRuntimeClient with AWS Access Key ID and AWS Secret Key.
    /// </summary>
    /// <param name="modelId">The model to be used for chat completion.</param>
    /// <param name="awsAccessKeyId">AWS Access Key ID</param>
    /// <param name="awsSecretAccessKey">AWS Secret Access Key</param>
    /// <param name="region">The region to connect.</param>
    public BedrockTextEmbeddingGenerationService(string modelId, string awsAccessKeyId, string awsSecretAccessKey, RegionEndpoint region)
        : base(modelId, new AmazonBedrockRuntimeClient(awsAccessKeyId, awsSecretAccessKey, region))
    {
        this._attributesInternal.Add(AIServiceExtensions.ModelIdKey, modelId);
    }
    /// <summary>
    /// Initializes an instance of the BedrockTextEmbeddingGenerationService.
    /// Constructs AmazonBedrockRuntimeClient with AWS Access Key ID, AWS Secret Key and an
    /// AmazonBedrockRuntimeClient Configuration object.
    /// </summary>
    /// <param name="modelId">The model to be used for chat completion.</param>
    /// <param name="awsAccessKeyId">AWS Access Key ID</param>
    /// <param name="awsSecretAccessKey">AWS Secret Access Key</param>
    /// <param name="clientConfig">The AmazonBedrockRuntimeClient Configuration Object</param>
    public BedrockTextEmbeddingGenerationService(string modelId, string awsAccessKeyId, string awsSecretAccessKey, AmazonBedrockRuntimeConfig clientConfig)
        : base(modelId, new AmazonBedrockRuntimeClient(awsAccessKeyId, awsSecretAccessKey, clientConfig))
    {
        this._attributesInternal.Add(AIServiceExtensions.ModelIdKey, modelId);
    }
    /// <summary>
    /// Initializes an instance of the BedrockTextEmbeddingGenerationService.
    /// Constructs AmazonBedrockRuntimeClient with AWS Access Key ID and AWS Secret Key and AWS Session Token.
    /// </summary>
    /// <param name="modelId">The model to be used for chat completion.</param>
    /// <param name="awsAccessKeyId">AWS Access Key ID</param>
    /// <param name="awsSecretAccessKey">AWS Secret Access Key</param>
    /// <param name="awsSessionToken">AWS Session Token</param>
    public BedrockTextEmbeddingGenerationService(string modelId, string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken)
        : base(modelId, new AmazonBedrockRuntimeClient(awsAccessKeyId, awsSecretAccessKey, awsSessionToken))
    {
        this._attributesInternal.Add(AIServiceExtensions.ModelIdKey, modelId);
    }

    /// <summary>
    /// Initializes an instance of the BedrockTextEmbeddingGenerationService.
    /// Constructs AmazonBedrockRuntimeClient with AWS Access Key ID and AWS Secret Key and AWS Session Token.
    /// </summary>
    /// <param name="modelId">The model to be used for chat completion.</param>
    /// <param name="awsAccessKeyId">AWS Access Key ID</param>
    /// <param name="awsSecretAccessKey">AWS Secret Access Key</param>
    /// <param name="awsSessionToken">AWS Session Token</param>
    /// <param name="region">The region to connect.</param>
    public BedrockTextEmbeddingGenerationService(string modelId, string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken, RegionEndpoint region)
        : base(modelId, new AmazonBedrockRuntimeClient(awsAccessKeyId, awsSecretAccessKey, awsSessionToken, region))
    {
        this._attributesInternal.Add(AIServiceExtensions.ModelIdKey, modelId);
    }
    /// <summary>
    /// Initializes an instance of the BedrockTextEmbeddingGenerationService.
    /// Constructs AmazonBedrockRuntimeClient with AWS Access Key ID, AWS Secret Key and an
    /// AmazonBedrockRuntimeClient Configuration object.
    /// </summary>
    /// <param name="modelId">The model to be used for chat completion.</param>
    /// <param name="awsAccessKeyId">AWS Access Key ID</param>
    /// <param name="awsSecretAccessKey">AWS Secret Access Key</param>
    /// <param name="awsSessionToken">AWS Session Token</param>
    /// <param name="clientConfig">The AmazonBedrockRuntimeClient Configuration Object</param>
    public BedrockTextEmbeddingGenerationService(string modelId, string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken, AmazonBedrockRuntimeConfig clientConfig)
        : base(modelId, new AmazonBedrockRuntimeClient(awsAccessKeyId, awsSecretAccessKey, awsSessionToken, clientConfig))
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
