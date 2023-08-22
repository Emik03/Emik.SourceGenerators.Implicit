// SPDX-License-Identifier: MPL-2.0
namespace Emik.SourceGenerators.Implicit;

/// <summary>Contains various extractors for <see cref="ISymbol"/> and its derivatives.</summary>
static class SymbolExtractors
{
    /// <summary>Gets all of the type arguments if the only parameter is a tuple.</summary>
    /// <param name="symbol">The symbol to use.</param>
    /// <returns>The enumeration of the type arguments if the only parameter is a tuple.</returns>
    public static IEnumerable<ITypeSymbol> Destructure(this IMethodSymbol symbol) =>
        symbol.Parameters is [{ Type: INamedTypeSymbol { Name: nameof(ValueTuple), TypeArguments: var args } }]
            ? args
            : symbol.ParameterTypes();

    /// <summary>Gets all of the parameter types.</summary>
    /// <param name="symbol">The symbol to use.</param>
    /// <returns>The enumeration of the parameter types.</returns>
    public static IEnumerable<ITypeSymbol> ParameterTypes(this IMethodSymbol symbol) =>
        symbol.Parameters.Select(x => x.Type);
}
