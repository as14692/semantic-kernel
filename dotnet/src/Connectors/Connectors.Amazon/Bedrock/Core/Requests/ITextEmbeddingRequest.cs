// Copyright (c) Microsoft. All rights reserved.

namespace Connectors.Amazon.Core.Requests;

public interface ITextEmbeddingRequest
{
    /// <summary>
    /// The prompt given to Bedrock model.
    /// </summary>
    string InputText { get; }
}
