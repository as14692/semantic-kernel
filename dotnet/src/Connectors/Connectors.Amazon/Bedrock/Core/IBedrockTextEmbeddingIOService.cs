// Copyright (c) Microsoft. All rights reserved.

using System;
using Amazon.BedrockRuntime.Model;

namespace Microsoft.SemanticKernel.Connectors.Amazon.Core;

internal interface IBedrockTextEmbeddingIOService
{
    /// <summary>
    /// Builds InvokeModelRequest Body parameter to be serialized for text embedding generation.
    /// </summary>
    /// <param name="data"></param>
    /// /// <param name="modelId"></param>
    /// <returns></returns>
    internal object GetEmbeddingRequestBody(string data, string modelId);

    /// <summary>
    /// Gets the response from the invoke model to build text embedding generation response structure.
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    internal ReadOnlyMemory<float> GetEmbeddingResponseBody(InvokeModelResponse response);
}
