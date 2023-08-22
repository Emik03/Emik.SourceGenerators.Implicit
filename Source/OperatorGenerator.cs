// SPDX-License-Identifier: MPL-2.0
namespace Emik.SourceGenerators.Implicit;

/// <summary>The source generator that implements implicit operators.</summary>
#pragma warning disable RS1038
[Generator]
#pragma warning restore RS1038
public sealed class OperatorGenerator : ISourceGenerator
{
    /// <inheritdoc />
    void ISourceGenerator.Execute(GeneratorExecutionContext context) =>
#if DEBUG
        new BadLogger().Try(Go, context).Dispose();
#else
        Go(context);
#endif

    /// <inheritdoc />
    void ISourceGenerator.Initialize(GeneratorInitializationContext context) { }

    static void Go(GeneratorExecutionContext context) =>
        context
           .Compilation
           .GetSymbolsWithName(_ => true, cancellationToken: context.CancellationToken)
           .OfType<INamedTypeSymbol>()
           .Where(SymbolPredicates.IsCandidate)
           .Select(x => (x, Source: x.Source(context.Compilation)))
           .Where(x => x.Source is not null)
           .Select(x => (HintName: x.x.HintName(), x.Source))
           .Lazily(x => context.AddSource(x.HintName, x.Source ?? throw Unreachable))
           .Enumerate();
}
