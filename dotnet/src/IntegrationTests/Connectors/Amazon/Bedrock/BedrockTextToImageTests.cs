// Copyright (c) Microsoft. All rights reserved.

using System.Threading.Tasks;
using Connectors.Amazon.Extensions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TextToImage;
using Xunit;

namespace SemanticKernel.IntegrationTests.Connectors.Amazon.Bedrock;

public class BedrockTextToImageTests
{
    [Theory]
    [InlineData("stability.stable-diffusion-xl-v1")]
    public async Task TextToImageReturnsValidResponseAsync(string modelId)
    {
        var textToImageKernel = Kernel.CreateBuilder()
            .AddBedrockTextToImageService(modelId)
            .Build();

        var textToImageService = textToImageKernel.GetRequiredService<ITextToImageService>();
        var result = await textToImageService.GenerateImageAsync("dog", 256, 256).ConfigureAwait(true);
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }
}
