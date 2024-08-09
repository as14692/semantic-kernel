﻿// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

namespace Microsoft.SemanticKernel.Connectors.Amazon.Core;

/// <summary>
/// Penalty struct for AI21 Jurassic. Accessed by both the execution settings and request object so must be kept public.
/// </summary>
[Serializable]

public class AI21JurassicPenalties
{
    /// <summary>
    /// Scale of the penalty.
    /// </summary>
    [JsonPropertyName("scale")]
    internal double Scale { get; set; }

    /// <summary>
    /// Whether to apply penalty to white spaces.
    /// </summary>
    [JsonPropertyName("applyToWhitespaces")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    internal bool? ApplyToWhitespaces { get; set; }

    /// <summary>
    /// Whether to apply penalty to punctuation.
    /// </summary>
    [JsonPropertyName("applyToPunctuations")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    internal bool? ApplyToPunctuations { get; set; }

    /// <summary>
    /// Whether to apply penalty to numbers.
    /// </summary>
    [JsonPropertyName("applyToNumbers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    internal bool? ApplyToNumbers { get; set; }

    /// <summary>
    /// Whether to apply penalty to stop words.
    /// </summary>
    [JsonPropertyName("applyToStopwords")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    internal bool? ApplyToStopwords { get; set; }

    /// <summary>
    /// Whether to apply penalty to emojis.
    /// </summary>
    [JsonPropertyName("applyToEmojis")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    internal bool? ApplyToEmojis { get; set; }
}