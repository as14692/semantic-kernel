﻿// Copyright (c) Microsoft. All rights reserved.

using Amazon.BedrockRuntime;
using Connectors.Amazon.Bedrock.Services;
using Connectors.Amazon.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Amazon.Services;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.TextGeneration;
using Microsoft.SemanticKernel.TextToImage;

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
                var logger = services.GetService<ILoggerFactory>();
                return new BedrockChatCompletionService(modelId, bedrockApi, logger);
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
        builder.Services.TryAddAWSService<IAmazonBedrockRuntime>();
        builder.Services.AddSingleton<IChatCompletionService>(services =>
        {
            try
            {
                var bedrockRuntime = services.GetRequiredService<IAmazonBedrockRuntime>();
                var logger = services.GetService<ILoggerFactory>();
                return new BedrockChatCompletionService(modelId, bedrockRuntime, logger);
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
                var logger = services.GetService<ILoggerFactory>();
                return new BedrockTextGenerationService(modelId, bedrockApi, logger);
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
        builder.Services.TryAddAWSService<IAmazonBedrockRuntime>();
        builder.Services.AddSingleton<ITextGenerationService>(services =>
        {
            try
            {
                var logger = services.GetService<ILoggerFactory>();
                var bedrockRuntime = services.GetRequiredService<IAmazonBedrockRuntime>();
                return new BedrockTextGenerationService(modelId, bedrockRuntime, logger);
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
    /// <returns></returns>
    public static IKernelBuilder AddBedrockTextEmbeddingGenerationService(
        this IKernelBuilder builder,
        string modelId,
        IAmazonBedrockRuntime bedrockApi)
    {
        builder.Services.AddSingleton<ITextEmbeddingGenerationService>(_ =>
        {
            try
            {
                var logger = services.GetService<ILoggerFactory>();
                return new BedrockTextEmbeddingGenerationService(modelId, bedrockApi, logger);
            }
            catch (Exception ex)
            {
                throw new KernelException($"An error occurred while initializing the BedrockTextEmbeddingGenerationService: {ex.Message}", ex);
            }
        });

        return builder;
    }

    /// <summary>
    /// Add Amazon Bedrock Text Embedding Generation service to the kernel builder using new AmazonBedrockRuntimeClient().
    /// </summary>
    /// <param name="builder">The kernel builder.</param>
    /// <param name="modelId">The model for chat completion.</param>
    /// <returns></returns>
    public static IKernelBuilder AddBedrockTextEmbeddingGenerationService(
        this IKernelBuilder builder,
        string modelId)
    {
        // Add IAmazonBedrockRuntime service client to the DI container
        builder.Services.AddAWSService<IAmazonBedrockRuntime>();
        builder.Services.AddSingleton<ITextEmbeddingGenerationService>(services =>
        {
            try
            {
                var bedrockRuntime = services.GetRequiredService<IAmazonBedrockRuntime>();
                var logger = services.GetService<ILoggerFactory>();
                return new BedrockTextEmbeddingGenerationService(modelId, bedrockRuntime, logger);
            }
            catch (Exception ex)
            {
                throw new KernelException($"An error occurred while initializing the BedrockTextEmbeddingGenerationService: {ex.Message}", ex);
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
    /// <returns></returns>
    public static IKernelBuilder AddBedrockTextToImageService(
        this IKernelBuilder builder,
        string modelId,
        IAmazonBedrockRuntime bedrockApi)
    {
        builder.Services.AddSingleton<ITextToImageService>(_ =>
        {
            try
            {
                return new BedrockTextToImageService(modelId, bedrockApi);
            }
            catch (Exception ex)
            {
                throw new KernelException($"An error occurred while initializing the BedrockTextToImageService: {ex.Message}", ex);
            }
        });

        return builder;
    }
    /// <summary>
    /// Kernel extension for text to image.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="modelId"></param>
    /// <returns></returns>
    /// <exception cref="KernelException"></exception>
    public static IKernelBuilder AddBedrockTextToImageService(
        this IKernelBuilder builder,
        string modelId)
    {
        // Add IAmazonBedrockRuntime service client to the DI container
        builder.Services.AddAWSService<IAmazonBedrockRuntime>();
        builder.Services.AddSingleton<ITextToImageService>(services =>
        {
            try
            {
                var bedrockRuntime = services.GetRequiredService<IAmazonBedrockRuntime>();
                return new BedrockTextToImageService(modelId, bedrockRuntime);
            }
            catch (Exception ex)
            {
                throw new KernelException($"An error occurred while initializing the BedrockTextToImageService: {ex.Message}", ex);
            }
        });

        return builder;
    }
}
