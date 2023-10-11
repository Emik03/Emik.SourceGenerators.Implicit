// SPDX-License-Identifier: MPL-2.0
namespace Emik.SourceGenerators.Implicit;

/// <summary>Generates the attribute needed to use this analyzer.</summary>
[Generator]
public sealed class AttributeGenerator() : FixedGenerator(
    "Emik.NoImplicitOperatorAttribute",
    $$"""
    namespace Emik
    {
        /// <summary>
        /// Prevents the analyzer from generating the implicit conversion into this constructor.
        /// </summary>
        [global::System.AttributeUsage(global::System.AttributeTargets.Class | global::System.AttributeTargets.Constructor | global::System.AttributeTargets.Struct)]
        {{Annotation}}
        internal sealed class NoImplicitOperatorAttribute : global::System.Attribute { }
    }
    """
);
