// SPDX-License-Identifier: MPL-2.0
Cylinder a = (2, 4); // Should not error.
Cylinder b = ("asf", "dfg"); // Should not error.
Cylinder c = "asf"; // Should error.
Cylinder d = 123; // Should error.

#pragma warning disable 0219, CA1819, IDE0059, MA0025, MA0110, RCS1085

partial class Cylinder(int i, int r)
{
    [method: NoImplicitOperator]
    public Cylinder(int i)
        : this(i, i) { }

    [method: NoImplicitOperator]
    public Cylinder((string, string) i)
        : this(0, 0) { }

    [method: NoImplicitOperator]
    internal Cylinder(string i)
        : this(0, 0) { }

    public static implicit operator Cylinder((string, string) i) => throw Unreachable;
}
