// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.Runtime.Endpoints;
using Connectors.Amazon.Bedrock.Services;
using Connectors.Amazon.Models.Stability;
using Microsoft.SemanticKernel.Services;
using Microsoft.SemanticKernel.TextToImage;
using Moq;
using Xunit;

namespace Microsoft.SemanticKernel.Connectors.Amazon.UnitTests;

/// <summary>
/// Unit tests for Text to Image service.
/// </summary>
public class BedrockTextToImageServiceTests
{
    /// <summary>
    /// Checks that modelID is added to the list of service attributes when service is registered.
    /// </summary>
    [Fact]
    public void AttributesShouldContainModelId()
    {
        // Arrange & Act
        string modelId = "stability.stable-diffusion-xl-v1";
        var mockBedrockApi = new Mock<IAmazonBedrockRuntime>();
        var service = new BedrockTextToImageService(modelId, mockBedrockApi.Object);

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
        string modelId = "stability.stable-diffusion-xl-v1";
        var mockBedrockApi = new Mock<IAmazonBedrockRuntime>();
        var textToImageResponse = new StableDiffusionResponse.StableDiffusionInvokeResponse()
        {
            Result = "success",
            Artifacts = new List<StableDiffusionResponse.StableDiffusionInvokeResponse.Artifact>
            {
                new()
                {
                    Seed = 123456,
                    Base64 = "iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAAAXNSR0IArs4c6QAAABxpRE9UAAAAAgAAAAAAAAACAAAAKADOAAcAAAABAAAAAABiQOuKAAABvElEQVRIDe2VsUtCURTHv+duQM7UBCto6a6tQXqBkRqENbUENBoUBDktQsQ/oCBaj1kcmyQJAnUgCGqIMt0amvqD/ah4eO7xothUN/X3e37n93G+9z3nPRfAuwWLJCWbUKEBAxrEgRT1NzJfXj9ksTC0f7qCzEyOCANEAAYwQQgAE4uKjazF3dY+XKUsLl4+w9YlQIAEFm2ePjLAlNcJtYXkyUA45TGC35rFGKSSXNdOR861SDjL+2ViwSJmRKQv5EaYkTZCp5/a65deP1WR/ZWVmIsEMDnci33XKslC+hMdMTbz3UEa6WGnXYiFwl45dYZKMrPh0dxCSzpH6ONQm/HrM9/aSuoxydt7TQTmNBk5+PTmxElpH4G2q73DK7pnrDFUilJJZq3jXmvjHp2L6i6YxJvMTaSqBVujMvjVlWPamuyCcn/GPAt6RfggwXcbNjNX3F3rR1Dw92ukb91qb9bPD/vZd/2SJHXKMhOIhUKyE1jI3VthwEJOQqGqVFQb2B5q19OSWqVq3acPrgnws+sxH2CFgAABBNCBBwQIwEAAAJK4DwVL45iv1p6T3y4GyKMOj3gPSbhcpHwGU/P7wW87Uyu/lU3/pQADAJ9Dr2wDR7wWAAAAAElFTkSuQmCC",
                    FinishReason = "SUCCESS"
                }
            }
        };
        mockBedrockApi.Setup(m => m.DetermineServiceOperationEndpoint(It.IsAny<InvokeModelRequest>()))
            .Returns(new Endpoint("https://bedrock-runtime.us-east-1.amazonaws.com")
            {
                URL = "https://bedrock-runtime.us-east-1.amazonaws.com"
            });
        mockBedrockApi.Setup(m => m.InvokeModelAsync(It.IsAny<InvokeModelRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InvokeModelResponse
            {
                Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(textToImageResponse))),
                ContentType = "application/json"
            });
        var kernel = Kernel.CreateBuilder().AddBedrockTextToImageService(modelId, mockBedrockApi.Object).Build();
        var service = kernel.GetRequiredService<ITextToImageService>();

        // Act
        var result = await service.GenerateImageAsync("dog", 256, 256).ConfigureAwait(true);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(textToImageResponse.Artifacts[0].Base64, result);
    }
}
