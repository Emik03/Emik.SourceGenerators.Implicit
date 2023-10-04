// SPDX-License-Identifier: MPL-2.0
namespace Emik.SourceGenerators.Implicit;

using static SymbolDisplayFormat;

/// <summary>
/// Provides a handler used by the language compiler to process interpolated strings into <see cref="string"/> instances.
/// </summary>
/// <param name="literalLength">
/// The number of constant characters outside of interpolation expressions in the interpolated string.
/// </param>
/// <param name="formattedCount">The number of interpolation expressions in the interpolated string.</param>
// ReSharper disable ConvertClosureToMethodGroup
[InterpolatedStringHandler]
readonly ref struct ParameterizedInterpolatedStringHandler(int literalLength, int formattedCount)
{
    readonly StringBuilder _builder = new(literalLength + formattedCount);

    /// <summary>Writes the specified string to the handler.</summary>
    /// <param name="value">The string to write.</param>
    public void AppendLiteral([StringSyntax("C#")] string? value) => _builder.Append(value);

    /// <summary>Writes the specified value to the handler.</summary>
    /// <param name="value">The value to write.</param>
    public void AppendFormatted([StringSyntax("C#")] string value) => AppendLiteral(value);

    /// <summary>Writes the specified value to the handler.</summary>
    /// <param name="value">The value to write.</param>
    /// <param name="format">The format string.</param>
    public void AppendFormatted(IList<IParameterSymbol> value, string? format = null) =>
        AppendLiteral(
            format is null ?
                value is [var x] ? x.Name : value.Select(x => $"tuple.{x.Name}").Conjoin() :
                value is [var y] ? Show(y) : $"({value.Select(x => Show(x)).Conjoin()}) tuple"
        );

    /// <summary>Writes the specified value to the handler.</summary>
    /// <param name="value">The value to write.</param>
    public void AppendFormatted(IEnumerable<string> value) => AppendLiteral(value.Conjoin("\n").TrimEnd());

    /// <summary>Writes the specified value to the handler.</summary>
    /// <param name="value">The value to write.</param>
    /// <param name="alignment">The indent.</param>
    public void AppendFormatted(IEnumerable<ITypeSymbol> value, int? alignment = null) =>
        AppendLiteral(value.Select(x => Show(x, alignment is null)).Conjoin());

    /// <summary>Writes the specified value to the handler.</summary>
    /// <param name="value">The value to write.</param>
    /// <param name="alignment">The indent.</param>
    /// <param name="format">The format string.</param>
    public void AppendFormatted(ISymbol value, int? alignment = null, string? format = null) =>
        AppendLiteral(Show(value, alignment is null, format is null));

    /// <inheritdoc />
    public override string ToString() => $"{_builder}";

    static string Show(IParameterSymbol x) => $"{Show(x.Type)} {x.Name}";

    static string Show(ISymbol value, bool keepBrackets = true, bool fullyQualified = true) =>
        value is INamespaceSymbol ? value.Name :
        (fullyQualified ? FullyQualifiedFormat : MinimallyQualifiedFormat) is var format &&
        value.ToDisplayString(format) is var output &&
        keepBrackets ? output : output.Replace('<', '{').Replace('>', '}');
}
