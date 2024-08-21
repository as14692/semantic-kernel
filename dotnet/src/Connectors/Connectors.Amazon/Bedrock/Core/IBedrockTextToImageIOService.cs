// Copyright (c) Microsoft. All rights reserved.

using Amazon.BedrockRuntime.Model;

namespace Microsoft.SemanticKernel.Connectors.Amazon.Core;

/// <summary>
/// Bedrock input-output service to build the request and response bodies as required by the given model.
/// </summary>
internal interface IBedrockTextToImageIOService
{
    /// <summary>
    /// Get Invoke Request Body For TextToImage service.
    /// </summary>
    /// <param name="modelId"></param>
    /// <param name="description"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="executionSettings"></param>
    /// <returns></returns>
    internal object GetInvokeRequestBodyForTextToImage(
        string modelId,
        string description,
        int width,
        int height,
        PromptExecutionSettings? executionSettings = null);

    /// <summary>
    /// Get the Base64 string output image result from the InvokeModelResponse for TextToImage service.
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    internal string GetInvokeResponseForImage(InvokeModelResponse response);
}
