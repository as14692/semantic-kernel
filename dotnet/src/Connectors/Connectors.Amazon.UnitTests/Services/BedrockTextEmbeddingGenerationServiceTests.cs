// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.Runtime.Endpoints;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Amazon.Core;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Services;
using Moq;
using Xunit;

namespace Connectors.Amazon.UnitTests.Services;

/// <summary>
/// Unit tests for Text Embedding Generation Service.
/// </summary>
public class BedrockTextEmbeddingGenerationServiceTests
{
    /// <summary>
    /// Checks that modelID is added to the list of service attributes when service is registered.
    /// </summary>
    [Fact]
    public void AttributesShouldContainModelId()
    {
        // Arrange & Act
        string modelId = "cohere.embed-english-v3";
        var mockBedrockApi = new Mock<IAmazonBedrockRuntime>();
        var kernel = Kernel.CreateBuilder().AddBedrockTextEmbeddingGenerationService(modelId, mockBedrockApi.Object).Build();
        var service = kernel.GetRequiredService<ITextEmbeddingGenerationService>();

        // Assert
        Assert.Equal(modelId, service.Attributes[AIServiceExtensions.ModelIdKey]);
    }

    /// <summary>
    /// Checks that GetChatMessageContentsAsync calls and correctly handles outputs from ConverseAsync.
    /// </summary>
    [Fact]
    public async Task GenerateEmbeddingsAsyncShouldReturnEmbeddingsAsync()
    {
        // Arrange
        string modelId = "amazon.titan-embed-text-v2:0";
        var mockBedrockApi = new Mock<IAmazonBedrockRuntime>();
        var embeddingResponse = new TitanEmbeddingResponse
        {
            Embedding = new List<float> { 0.1f, -0.2f, 0.3f, -0.4f },
            InputTextTokenCount = 10
        };
        mockBedrockApi.Setup(m => m.DetermineServiceOperationEndpoint(It.IsAny<InvokeModelRequest>()))
            .Returns(new Endpoint("https://bedrock-runtime.us-east-1.amazonaws.com")
            {
                URL = "https://bedrock-runtime.us-east-1.amazonaws.com"
            });
        mockBedrockApi.Setup(m => m.InvokeModelAsync(It.IsAny<InvokeModelRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InvokeModelResponse
            {
                Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(embeddingResponse))),
                ContentType = "application/json"
            });
        var kernel = Kernel.CreateBuilder().AddBedrockTextEmbeddingGenerationService(modelId, mockBedrockApi.Object).Build();
        var service = kernel.GetRequiredService<ITextEmbeddingGenerationService>();
        var inputText = "This is a sample text.";

        // Act
        var result = await service.GenerateEmbeddingsAsync(new[] { inputText }).ConfigureAwait(true);

        // Assert
        Assert.Single(result);
        Assert.Equal(new ReadOnlyMemory<float>(embeddingResponse.Embedding.ToArray()), result[0]);
    }
}
