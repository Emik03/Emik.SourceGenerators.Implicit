# Emik.SourceGenerators.Implicit

[![NuGet package](https://img.shields.io/nuget/v/Emik.SourceGenerators.Implicit.svg?logo=NuGet)](https://www.nuget.org/packages/Emik.SourceGenerators.Implicit)
[![License](https://img.shields.io/github/license/Emik03/Emik.SourceGenerators.Implicit.svg?style=flat)](https://github.com/Emik03/Emik.SourceGenerators.Implicit/blob/main/LICENSE)

Source-generates implicit conversion operators that map to the respective constructors.

This project has a dependency to [Emik.Morsels](https://github.com/Emik03/Emik.Morsels), if you are building this
project, refer to its [README](https://github.com/Emik03/Emik.Morsels/blob/main/README.md) first.

---

- [Example](#example)
- [Contribute](#contribute)
- [License](#license)

---

## Example

```csharp
A singularValuesWork = 1; // Valid!
A tuplesWorkToo = (2, 3); // Valid!

// Won't generate: annotated NoImplicitOperator.
// A butThisDoesNot = (4, 5, 6);

// Won't generate: would change visibility of constructor.
// A andNeitherDoesThis = (7, 8, 9, 10); 

// Won't generate: annotated NoImplicitOperator.
// B andNeitherDoesThisToo = 11; 
// B orThis = (12, 13);

public class A(int i)
{
    public A(int i, int j)
       : this(0) { }

    [NoImplicitOperator]
    public A(int i, int j, int k)
       : this(0) { }

    private A(int i, int j, int k, int l)
       : this(0) { }
}

[NoImplicitOperator]
public class B(int i)
{
    public B(int i, int j)
       : this(0) { }
}
```

## Contribute

Issues and pull requests are welcome to help this repository be the best it can be.

## License

This repository falls under the [MPL-2 license](https://www.mozilla.org/en-US/MPL/2.0/).
