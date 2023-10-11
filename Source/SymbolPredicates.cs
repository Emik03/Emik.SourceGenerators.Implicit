// SPDX-License-Identifier: MPL-2.0
namespace Emik.SourceGenerators.Implicit;

using static Accessibility;

/// <summary>Contains various predicates for <see cref="ISymbol"/> and its derivatives.</summary>
static class SymbolPredicates
{
    /// <summary>Determines whether the symbol can be in an implicit operator.</summary>
    /// <param name="method">The symbol to check.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="method"/>
    /// can be in an implicit operator, otherwise; <see langword="false"/>.
    /// </returns>
    [Pure]
    public static bool CanBeInImplicitOperator(this IMethodSymbol method) =>
        method.Parameters is [var y] ? y.Type.IsInterface() : method.Parameters.Any(x => !x.Type.CanBeGeneric());

    /// <summary>Determines whether the symbol has zero or one parameter of the specified parameter.</summary>
    /// <param name="single">The comparison when there is one parameter.</param>
    /// <param name="method">The symbol to check.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="method"/>
    /// has 0 parameters, or 1 parameter of type <paramref name="single"/>.
    /// </returns>
    [Pure]
    public static bool HasEmptyParametersOrSingleInterfaceOrSingleSelf(this ITypeSymbol single, IMethodSymbol method) =>
        method.Parameters is [{ Type: var type }]
            ? type.IsInterface() || TypeSymbolComparer.Equal(single, type)
            : method.Parameters.IsEmpty;

    /// <summary>Determines whether the symbol has zero or one parameter of the specified parameter.</summary>
    /// <param name="single">The comparison when there is one parameter.</param>
    /// <param name="method">The symbol to check.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="method"/>
    /// has 0 parameters, or 1 parameter of type <paramref name="single"/>.
    /// </returns>
    [Pure]
    public static bool HasSameParameters(this ITypeSymbol single, IMethodSymbol method) =>
        single
           .GetMembers()
           .OfType<IMethodSymbol>()
           .Where(x => x.Name is "op_Implicit" or "op_Explicit")
           .Any(method.ParameterTypeSequenceEqual);

    /// <summary>Determines whether the symbol is a potential candidate for extension.</summary>
    /// <param name="symbol">The symbol to check.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="symbol"/> is a candidate.
    /// </returns>
    [Pure]
    public static bool IsCandidate(this INamedTypeSymbol symbol) =>
        symbol is { IsAbstract: false, IsStatic: false } &&
        symbol.IsCompletelyPartial();

    /// <summary>Determines whether the right-hand side is relatively accessible to the left.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="right"/> is relatively
    /// accessible to <paramref name="left"/>, otherwise; <see langword="false"/>.
    /// </returns>
    public static bool IsRelativelyAccessible(this Accessibility left, Accessibility right) =>
        left switch
        {
            _ when right is Private or ProtectedAndInternal or Protected => false,
            Public when right is Internal or ProtectedOrInternal => false,
            _ => true,
        };

    /// <summary>Determines whether the method can be exposed in a public function from the type.</summary>
    /// <param name="type">The type that declares the method.</param>
    /// <param name="method">The method to check its accessibility of.</param>
    /// <returns>
    /// The value <see langword="true"/> if the parameter <paramref name="method"/> has greater than
    /// or equal accessibility to <paramref name="type"/>, otherwise; <see langword="false"/>.
    /// </returns>
    public static bool IsRelativelyAccessible(this INamedTypeSymbol type, IMethodSymbol method) =>
        !method.IsObsolete() &&
        method.DeclaredAccessibility is var accessibility &&
        type
           .FindSmallPathToNull(x => x.ContainingType)
           .All(x => x.DeclaredAccessibility.IsRelativelyAccessible(accessibility));

    /// <summary>Determines whether both methods have the same parameter types.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>
    /// The value <see langword="true"/> if both parameters have the
    /// same parameter types, otherwise; <see langword="false"/>.
    /// </returns>
    public static bool ParameterTypeSequenceEqual(this IMethodSymbol left, IMethodSymbol right) =>
        SequenceEqual(left, right.Destructure()) || SequenceEqual(left, right.ParameterTypes());

    static bool SequenceEqual(IMethodSymbol left, IEnumerable<ITypeSymbol> right) =>
        left.ParameterTypes().SequenceEqual(right, TypeSymbolComparer.Default);
}
