// Copyright (c) Microsoft. All rights reserved.

using Amazon.BedrockRuntime;
using Amazon.Runtime;
using Connectors.Amazon.Bedrock.Services;
using Connectors.Amazon.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Amazon.Services;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.TextGeneration;

namespace Connectors.Amazon.Extensions;

/// <summary>
/// Extensions for adding Bedrock services to the application.
/// </summary>
public static class BedrockKernelBuilderExtensions
{
    /// <summary>
    /// Add Amazon Bedrock Chat Completion service to the kernel builder using IAmazonBedrockRuntime object.
    /// </summary>
    /// <param name="builder">The kernel builder.</param>
    /// <param name="modelId">The model for chat completion.</param>
    /// <param name="bedrockApi">The IAmazonBedrockRuntime to run inference using the respective model.</param>
    /// <returns></returns>
    public static IKernelBuilder AddBedrockChatCompletionService(
        this IKernelBuilder builder,
        string modelId,
        IAmazonBedrockRuntime bedrockApi)
    {
        builder.Services.AddSingleton<IChatCompletionService>(services =>
        {
            try
            {
                return new BedrockChatCompletionService(modelId, bedrockApi);
            }
            catch (Exception ex)
            {
                throw new KernelException($"An error occurred while initializing the BedrockChatCompletionService: {ex.Message}", ex);
            }
        });

        return builder;
    }

    /// <summary>
    /// Add Amazon Bedrock Chat Completion service to the kernel builder using new AmazonBedrockRuntimeClient().
    /// </summary>
    /// <param name="builder">The kernel builder.</param>
    /// <param name="modelId">The model for chat completion.</param>
    /// <returns></returns>
    public static IKernelBuilder AddBedrockChatCompletionService(
        this IKernelBuilder builder,
        string modelId)
    {
        // Add IAmazonBedrockRuntime service client to the DI container
        builder.Services.AddAWSService<IAmazonBedrockRuntime>();
        builder.Services.AddSingleton<IChatCompletionService>(services =>
        {
            try
            {
                var bedrockRuntime = services.GetRequiredService<IAmazonBedrockRuntime>();
                return new BedrockChatCompletionService(modelId, bedrockRuntime);
            }
            catch (Exception ex)
            {
                throw new KernelException($"An error occurred while initializing the BedrockChatCompletionService: {ex.Message}", ex);
            }
        });

        return builder;
    }
    /// <summary>
    /// Add Amazon Bedrock Text Generation service to the kernel builder using IAmazonBedrockRuntime object.
    /// </summary>
    /// <param name="builder">The kernel builder.</param>
    /// <param name="modelId">The model for text generation.</param>
    /// <param name="bedrockApi">The IAmazonBedrockRuntime to run inference using the respective model.</param>
    /// <returns></returns>
    public static IKernelBuilder AddBedrockTextGenerationService(
        this IKernelBuilder builder,
        string modelId,
        IAmazonBedrockRuntime bedrockApi)
    {
        builder.Services.AddSingleton<ITextGenerationService>(services =>
        {
            try
            {
                return new BedrockTextGenerationService(modelId, bedrockApi);
            }
            catch (Exception ex)
            {
                throw new KernelException($"An error occurred while initializing the BedrockTextGenerationService: {ex.Message}", ex);
            }
        });

        return builder;
    }
    /// <summary>
    /// Add Amazon Bedrock Text Generation service to the kernel builder using new AmazonBedrockRuntimeClient().
    /// </summary>
    /// <param name="builder">The kernel builder.</param>
    /// <param name="modelId">The model for text generation.</param>
    /// <returns></returns>
    public static IKernelBuilder AddBedrockTextGenerationService(
        this IKernelBuilder builder,
        string modelId)
    {
        // Add IAmazonBedrockRuntime service client to the DI container
        builder.Services.AddAWSService<IAmazonBedrockRuntime>();
        builder.Services.AddSingleton<ITextGenerationService>(services =>
        {
            try
            {
                var bedrockRuntime = services.GetRequiredService<IAmazonBedrockRuntime>();
                return new BedrockTextGenerationService(modelId, bedrockRuntime);
            }
            catch (Exception ex)
            {
                throw new KernelException($"An error occurred while initializing the BedrockTextGenerationService: {ex.Message}", ex);
            }
        });

        return builder;
    }

    /// <summary>
    /// Add Amazon Bedrock Text Embedding Generation service to the kernel builder using IAmazonBedrockRuntime object.
    /// </summary>
    /// <param name="builder">The kernel builder.</param>
    /// <param name="modelId">The model for chat completion.</param>
    /// <param name="bedrockApi">The IAmazonBedrockRuntime to run inference using the respective model.</param>
    /// <param name="serviceId">The optional service ID.</param>
    /// <returns></returns>
    public static IKernelBuilder AddBedrockTextEmbeddingGenerationService(
        this IKernelBuilder builder,
        string modelId,
        IAmazonBedrockRuntime bedrockApi,
        string? serviceId = null)
    {
        builder.Services.AddKeyedSingleton<IChatCompletionService>(serviceId, (_, _)
            => new BedrockChatCompletionService(modelId, bedrockApi));

        return builder;
    }

    /// <summary>
    /// Add Amazon Bedrock Text Embedding Generation service to the kernel builder using new AmazonBedrockRuntimeClient().
    /// </summary>
    /// <param name="builder">The kernel builder.</param>
    /// <param name="modelId">The model for chat completion.</param>
    /// <param name="serviceId">The optional service ID.</param>
    /// <returns></returns>
    public static IKernelBuilder AddBedrockTextEmbeddingGenerationService(
        this IKernelBuilder builder,
        string modelId,
        string? serviceId = null)
    {
        builder.Services.AddKeyedSingleton<ITextEmbeddingGenerationService>(serviceId, (_, _)
            => new BedrockTextEmbeddingGenerationService(modelId));

        return builder;
    }
    /// <summary>
    /// Add Amazon Bedrock Text Embedding Generation service to the kernel builder using new AmazonBedrockRuntimeClient().
    /// </summary>
    /// <param name="builder">The kernel builder.</param>
    /// <param name="modelId">The model for chat completion.</param>
    /// <param name="region">The region to connect.</param>
    /// <param name="serviceId">The optional service ID.</param>
    /// <returns></returns>
    public static IKernelBuilder AddBedrockTextEmbeddingGenerationService(
        this IKernelBuilder builder,
        string modelId,
        RegionEndpoint region,
        string? serviceId = null)
    {
        builder.Services.AddKeyedSingleton<IChatCompletionService>(serviceId, (_, _)
            => new BedrockChatCompletionService(modelId, region));

        return builder;
    }
    /// <summary>
    /// Add Amazon Bedrock Text Embedding Generation service to the kernel builder using new AmazonBedrockRuntimeClient().
    /// </summary>
    /// <param name="builder">The kernel builder.</param>
    /// <param name="modelId">The model for chat completion.</param>
    /// <param name="clientConfig">The AmazonBedrockRuntimeClient Configuration Object</param>
    /// <param name="serviceId">The optional service ID.</param>
    /// <returns></returns>
    public static IKernelBuilder AddBedrockTextEmbeddingGenerationService(
        this IKernelBuilder builder,
        string modelId,
        AmazonBedrockRuntimeConfig clientConfig,
        string? serviceId = null)
    {
        builder.Services.AddKeyedSingleton<IChatCompletionService>(serviceId, (_, _)
            => new BedrockChatCompletionService(modelId, clientConfig));

        return builder;
    }
    /// <summary>
    /// Add Amazon Bedrock Text Embedding Generation service to the kernel builder using AWSCredentials object.
    /// </summary>
    /// <param name="builder">The kernel builder.</param>
    /// <param name="modelId">The model for chat completion.</param>
    /// <param name="awsCredentials">A credentials object for AWS services.</param>
    /// <param name="serviceId">The optional service ID.</param>
    /// <returns></returns>
    public static IKernelBuilder AddBedrockTextEmbeddingGenerationService(
        this IKernelBuilder builder,
        string modelId,
        AWSCredentials awsCredentials,
        string? serviceId = null)
    {
        builder.Services.AddKeyedSingleton<IChatCompletionService>(serviceId, (_, _)
            => new BedrockChatCompletionService(modelId, awsCredentials));

        return builder;
    }
    /// <summary>
    /// Add Amazon Bedrock Text Embedding Generation service to the kernel builder using AWSCredentials object.
    /// </summary>
    /// <param name="builder">The kernel builder.</param>
    /// <param name="modelId">The model for chat completion.</param>
    /// <param name="awsCredentials">A credentials object for AWS services.</param>
    /// <param name="region">The region to connect.</param>
    /// <param name="serviceId">The optional service ID.</param>
    /// <returns></returns>
    public static IKernelBuilder AddBedrockTextEmbeddingGenerationService(
        this IKernelBuilder builder,
        string modelId,
        AWSCredentials awsCredentials,
        RegionEndpoint region,
        string? serviceId = null)
    {
        builder.Services.AddKeyedSingleton<IChatCompletionService>(serviceId, (_, _)
            => new BedrockChatCompletionService(modelId, awsCredentials, region));

        return builder;
    }
    /// <summary>
    /// Add Amazon Bedrock Text Embedding Generation service to the kernel builder using AWSCredentials object.
    /// </summary>
    /// <param name="builder">The kernel builder.</param>
    /// <param name="modelId">The model for chat completion.</param>
    /// <param name="awsCredentials">A credentials object for AWS services.</param>
    /// <param name="clientConfig">The AmazonBedrockRuntimeClient Configuration Object</param>
    /// <param name="serviceId">The optional service ID.</param>
    /// <returns></returns>
    public static IKernelBuilder AddBedrockTextEmbeddingGenerationService(
        this IKernelBuilder builder,
        string modelId,
        AWSCredentials awsCredentials,
        AmazonBedrockRuntimeConfig clientConfig,
        string? serviceId = null)
    {
        builder.Services.AddKeyedSingleton<IChatCompletionService>(serviceId, (_, _)
            => new BedrockChatCompletionService(modelId, awsCredentials, clientConfig));

        return builder;
    }
    /// <summary>
    /// Add Amazon Bedrock Text Embedding Generation service to the kernel builder using AWS Access Key ID and AWS Secret Access Key.
    /// </summary>
    /// <param name="builder">The kernel builder.</param>
    /// <param name="modelId">The model for chat completion.</param>
    /// <param name="awsAccessKeyId">AWS Access Key ID</param>
    /// <param name="awsSecretAccessKey">AWS Secret Access Key</param>
    /// <param name="serviceId">The optional service ID.</param>
    /// <returns></returns>
    public static IKernelBuilder AddBedrockTextEmbeddingGenerationService(
        this IKernelBuilder builder,
        string modelId,
        string awsAccessKeyId,
        string awsSecretAccessKey,
        string? serviceId = null)
    {
        builder.Services.AddKeyedSingleton<IChatCompletionService>(serviceId, (_, _)
            => new BedrockChatCompletionService(modelId, awsAccessKeyId, awsSecretAccessKey));

        return builder;
    }

    /// <summary>
    /// Add Amazon Bedrock Text Embedding Generation service to the kernel builder using AWS Access Key ID and AWS Secret Access Key.
    /// </summary>
    /// <param name="builder">The kernel builder.</param>
    /// <param name="modelId">The model for chat completion.</param>
    /// <param name="awsAccessKeyId">AWS Access Key ID</param>
    /// <param name="awsSecretAccessKey">AWS Secret Access Key</param>
    /// <param name="region">The region to connect.</param>
    /// <param name="serviceId">The optional service ID.</param>
    /// <returns></returns>
    public static IKernelBuilder AddBedrockTextEmbeddingGenerationService(
        this IKernelBuilder builder,
        string modelId,
        string awsAccessKeyId,
        string awsSecretAccessKey,
        RegionEndpoint region,
        string? serviceId = null)
    {
        builder.Services.AddKeyedSingleton<IChatCompletionService>(serviceId, (_, _)
            => new BedrockChatCompletionService(modelId, awsAccessKeyId, awsSecretAccessKey, region));

        return builder;
    }
    /// <summary>
    /// Add Amazon Bedrock Text Embedding Generation service to the kernel builder using AWS Access Key ID and AWS Secret Access Key and AmazonBedrockRuntimeClient Configuration object.
    /// </summary>
    /// <param name="builder">The kernel builder.</param>
    /// <param name="modelId">The model for chat completion.</param>
    /// <param name="awsAccessKeyId">AWS Access Key ID</param>
    /// <param name="awsSecretAccessKey">AWS Secret Access Key</param>
    /// <param name="clientConfig">The AmazonBedrockRuntimeClient Configuration Object</param>
    /// <param name="serviceId">The optional service ID.</param>
    /// <returns></returns>
    public static IKernelBuilder AddBedrockTextEmbeddingGenerationService(
        this IKernelBuilder builder,
        string modelId,
        string awsAccessKeyId,
        string awsSecretAccessKey,
        AmazonBedrockRuntimeConfig clientConfig,
        string? serviceId = null)
    {
        builder.Services.AddKeyedSingleton<IChatCompletionService>(serviceId, (_, _)
            => new BedrockChatCompletionService(modelId, awsAccessKeyId, awsSecretAccessKey, clientConfig));

        return builder;
    }
    /// <summary>
    /// Add Amazon Bedrock Text Embedding Generation service to the kernel builder using AWS Access Key ID and AWS Secret Access Key and AWS Session Token.
    /// </summary>
    /// <param name="builder">The kernel builder.</param>
    /// <param name="modelId">The model for chat completion.</param>
    /// <param name="awsAccessKeyId">AWS Access Key ID</param>
    /// <param name="awsSecretAccessKey">AWS Secret Access Key</param>
    /// <param name="awsSessionToken">AWS Session Token</param>
    /// <param name="serviceId">The optional service ID.</param>
    /// <returns></returns>
    public static IKernelBuilder AddBedrockTextEmbeddingGenerationService(
        this IKernelBuilder builder,
        string modelId,
        string awsAccessKeyId,
        string awsSecretAccessKey,
        string awsSessionToken,
        string? serviceId = null)
    {
        builder.Services.AddKeyedSingleton<IChatCompletionService>(serviceId, (_, _)
            => new BedrockChatCompletionService(modelId, awsAccessKeyId, awsSecretAccessKey, awsSessionToken));

        return builder;
    }
    /// <summary>
    /// Add Amazon Bedrock Text Embedding Generation service to the kernel builder using AWS Access Key ID and AWS Secret Access Key and AWS Session Token.
    /// </summary>
    /// <param name="builder">The kernel builder.</param>
    /// <param name="modelId">The model for chat completion.</param>
    /// <param name="awsAccessKeyId">AWS Access Key ID</param>
    /// <param name="awsSecretAccessKey">AWS Secret Access Key</param>
    /// <param name="awsSessionToken">AWS Session Token</param>
    /// <param name="region">The region to connect.</param>
    /// <param name="serviceId">The optional service ID.</param>
    /// <returns></returns>
    public static IKernelBuilder AddBedrockTextEmbeddingGenerationService(
        this IKernelBuilder builder,
        string modelId,
        string awsAccessKeyId,
        string awsSecretAccessKey,
        string awsSessionToken,
        RegionEndpoint region,
        string? serviceId = null)
    {
        builder.Services.AddKeyedSingleton<IChatCompletionService>(serviceId, (_, _)
            => new BedrockChatCompletionService(modelId, awsAccessKeyId, awsSecretAccessKey, awsSessionToken, region));

        return builder;
    }
    /// <summary>
    /// Add Amazon Bedrock Text Embedding Generation service to the kernel builder using AWS Access Key ID and AWS Secret Access Key and
    /// AWS Session Token and AmazonBedrockRuntimeClient Configuration Object.
    /// </summary>
    /// <param name="builder">The kernel builder.</param>
    /// <param name="modelId">The model for chat completion.</param>
    /// <param name="awsAccessKeyId">AWS Access Key ID</param>
    /// <param name="awsSecretAccessKey">AWS Secret Access Key</param>
    /// <param name="awsSessionToken">AWS Session Token</param>
    /// <param name="clientConfig">The AmazonBedrockRuntimeClient Configuration Object</param>
    /// <param name="serviceId">The optional service ID.</param>
    /// <returns></returns>
    public static IKernelBuilder AddBedrockTextEmbeddingGenerationService(
        this IKernelBuilder builder,
        string modelId,
        string awsAccessKeyId,
        string awsSecretAccessKey,
        string awsSessionToken,
        AmazonBedrockRuntimeConfig clientConfig,
        string? serviceId = null)
    {
        builder.Services.AddKeyedSingleton<IChatCompletionService>(serviceId, (_, _)
            => new BedrockChatCompletionService(modelId, awsAccessKeyId, awsSecretAccessKey, awsSessionToken, clientConfig));

        return builder;
    }
}
