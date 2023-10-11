// SPDX-License-Identifier: MPL-2.0
namespace Emik.SourceGenerators.Implicit;

/// <summary>The source generator that implements implicit operators.</summary>
[Generator]
public sealed class OperatorGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    void IIncrementalGenerator.Initialize(IncrementalGeneratorInitializationContext context)
    {
        var isDefined = context.CompilationProvider.Select(IsValueTupleDefined);

        var provider = context
           .SyntaxProvider
           .AgainstAttributeWithMetadataName(Of<AttributeGenerator>(), Is<BaseTypeDeclarationSyntax>, Target)
           .Filter()
           .Combine(isDefined)
           .Select(TryAddSource)
           .Filter()
           .Select(AddHintName);

        context.RegisterSourceOutput(provider, AddSource);
    }

    static bool IsValueTupleDefined(Compilation compilation, CancellationToken token)
    {
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var next in compilation.GetTypesByMetadataName($"{nameof(System)}.{nameof(ValueTuple)}"))
        {
            token.ThrowIfCancellationRequested();

            if (next.IsAccessible() || next.ContainingAssembly.Identity == compilation.Assembly.Identity)
                return true;
        }

        return false;
    }

    static INamedTypeSymbol? Target(SyntaxNode _, ISymbol symbol, SemanticModel model, CancellationToken __) =>
        symbol is INamedTypeSymbol { IsTupleType: false } x && x.IsCandidate() ? x : null;

    static (INamedTypeSymbol, string)? TryAddSource((INamedTypeSymbol Left, bool Right) tuple, CancellationToken _) =>
        tuple.Left.Source(tuple.Right) is { } source ? (tuple.Left, source) : null;

    static (string, string) AddHintName((INamedTypeSymbol Symbol, string Source) tuple, CancellationToken _) =>
        (tuple.Symbol.HintName(), tuple.Source);
}
