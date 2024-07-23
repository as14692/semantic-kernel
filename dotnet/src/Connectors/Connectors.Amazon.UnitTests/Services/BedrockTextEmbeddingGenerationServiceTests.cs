// Copyright (c) Microsoft. All rights reserved.

using System.Text;
using System.Text.Json;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Connectors.Amazon.Bedrock.Services;
using Connectors.Amazon.Models.Amazon;
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
        var service = new BedrockTextEmbeddingGenerationService(modelId, mockBedrockApi.Object);

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
        var embeddingResponse = new TitanTextEmbeddingResponse
        {
            Embedding = new List<float> { 0.1f, -0.2f, 0.3f, -0.4f },
            InputTextTokenCount = 10
        };
        mockBedrockApi.Setup(m => m.InvokeModelAsync(It.IsAny<InvokeModelRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InvokeModelResponse
            {
                Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(embeddingResponse))),
                ContentType = "application/json"
            });
        var service = new BedrockTextEmbeddingGenerationService(modelId, mockBedrockApi.Object);
        var inputText = "This is a sample text.";

        // Act
        var result = await service.GenerateEmbeddingsAsync(new[] { inputText }).ConfigureAwait(true);

        // Assert
        Assert.Single(result);
        Assert.Equal(new ReadOnlyMemory<float>(embeddingResponse.Embedding.ToArray()), result[0]);
    }
}
