// Copyright (c) Microsoft. All rights reserved.

using System.Threading.Tasks;
using Connectors.Amazon.Bedrock.Services;
using Connectors.Amazon.Extensions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Xunit;

namespace SemanticKernel.IntegrationTests.Connectors.Amazon.Bedrock;

public class BedrockTextEmbeddingGenerationTests
{
    [Theory]
    [InlineData("amazon.titan-embed-text-v1")]
    [InlineData("amazon.titan-embed-text-v2:0")]
    [InlineData("cohere.embed-english-v3")]
    public async Task TextEmbeddingGenerationReturnsValidResponseAsync(string modelId)
    {
        // Arrange
        var kernel = Kernel.CreateBuilder().AddBedrockTextEmbeddingGenerationService(modelId).Build();
        var service = kernel.GetRequiredService<ITextEmbeddingGenerationService>();
        var inputText = "LLM is Large Language Model.";

        // Act
        var result = await service.GenerateEmbeddingsAsync(new[] { inputText }).ConfigureAwait(true);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count > 0);
        Assert.True(result[0].Length > 0);
    }
}
