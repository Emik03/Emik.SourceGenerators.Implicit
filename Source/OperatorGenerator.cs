// SPDX-License-Identifier: MPL-2.0
namespace Emik.SourceGenerators.Implicit;

/// <summary>The source generator that implements implicit operators.</summary>
[Generator]
public sealed class OperatorGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    void IIncrementalGenerator.Initialize(IncrementalGeneratorInitializationContext context)
    {
        var typeProvider = context.SyntaxProvider.CreateSyntaxProvider(Is<BaseTypeDeclarationSyntax>, Target).Filter();
        var provider = context.CompilationProvider.Combine(typeProvider.Collect());
        context.RegisterSourceOutput(provider, Go);
    }

    static void Go(SourceProductionContext context, (Compilation Left, ImmutableArray<INamedTypeSymbol> Right) tuple)
    {
        var (compilation, types) = tuple;

        foreach (var type in types)
            if (type.Source(compilation) is { } source && type.HintName() is var hintName)
                context.AddSource(hintName, source);
    }

    static INamedTypeSymbol? Target(GeneratorSyntaxContext context, CancellationToken token) =>
        context.SemanticModel.GetSymbolInfo(context.Node, token).Symbol is INamedTypeSymbol { IsTupleType: false } x &&
        x.IsCandidate()
            ? x
            : null;
}
