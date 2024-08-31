// SPDX-License-Identifier: MPL-2.0
namespace Emik.SourceGenerators.Implicit;

/// <summary>The source generator that implements implicit operators.</summary>
[Generator]
public sealed class OperatorGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    void IIncrementalGenerator.Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context
           .SyntaxProvider
           .AgainstAttributeWithMetadataName(Of<AttributeGenerator>(), Is<BaseTypeDeclarationSyntax>, Target)
           .Filter()
           .Combine(context.CompilationProvider.Select(IsValueTupleDefined))
           .Select(TryAddSource)
           .Filter()
           .Select(AddHintName);

        context.RegisterSourceOutput(provider, AddSource);
    }

    /// <summary>Executes this source generation based on given input.</summary>
    /// <remarks><para>
    /// This API is unused within the library, but can be used by other libraries that reference
    /// this library to integrate this source generator however they wish. Do note that the API
    /// is subject to change, but the general premise of the function will remain the same.
    /// </para></remarks>
    /// <param name="named">The type to generate for.</param>
    /// <param name="hasValueTuple">
    /// If the compilation supports the <see cref="ValueTuple"/> type. Set this to <see langword="false"/> if an older
    /// framework is used without a polyfill of said type. Such older frameworks include <c>.NET Framework 4.6.2</c>
    /// or earlier, <c>.NET Standard 1.6</c> or earlier, <c>.NET Core 1.0</c>, and <c>.NET Core 1.1</c>.
    /// </param>
    /// <returns>
    /// The generated source. This contains the hint name, which indicates the name of the file, and the source,
    /// representing the contents of said file. This function will return <see langword="null"/> under a few
    /// circumstances. This includes the parameter <paramref name="named"/> being <see langword="null"/>, being a tuple
    /// type regardless of if it is a polyfill or not, not having any suitable constructors to wrap implicit operators
    /// around, not being fully <see langword="partial"/> which includes any types that contain the parameter
    /// <paramref name="named"/>.
    /// </returns>
    [Pure]
    public static GeneratedSource? Transform(INamedTypeSymbol? named, bool hasValueTuple = true) =>
        named is { IsTupleType: false } &&
        named.IsCandidate() &&
        TryAddSource((named, hasValueTuple), default) is { } tuple
            ? AddHintName(tuple, default)
            : null;

    static bool IsValueTupleDefined(Compilation compilation, CancellationToken token) =>
        compilation.GetTypesByMetadataName($"{nameof(System)}.{nameof(ValueTuple)}").Any();

    static INamedTypeSymbol? Target(SyntaxNode node, ISymbol symbol, SemanticModel model, CancellationToken token) =>
        symbol is INamedTypeSymbol { IsTupleType: false } x && x.IsCandidate() && node.IsFirst(symbol, token)
            ? x
            : null;

    static (INamedTypeSymbol, string)? TryAddSource((INamedTypeSymbol Left, bool Right) tuple, CancellationToken _) =>
        tuple.Left.Source(tuple.Right) is { } source ? (tuple.Left, source) : null;

    static (string, string) AddHintName((INamedTypeSymbol Symbol, string Source) tuple, CancellationToken _) =>
        (tuple.Symbol.HintName(), tuple.Source);
}
