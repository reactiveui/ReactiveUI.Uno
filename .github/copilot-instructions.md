# GitHub Copilot Instructions for ReactiveUI.Uno

## 🚨 VITAL PREREQUISITES - READ FIRST 🚨

**BEFORE MAKING ANY CODE CHANGES OR BUILD ATTEMPTS**, you MUST ensure these requirements are met. Failure to do so will result in redundant actions and wasted time.

### 1. .NET 9 SDK Requirement - MANDATORY

This project **REQUIRES** .NET 9 SDK (specifically version 9.0.304 or later). The project will NOT build with .NET 8 or earlier versions.

**Before any build or development work:**

```bash
# Check current .NET version
dotnet --version

# If not 9.0.304 or later, install .NET 9 SDK:
# Download from: https://dotnet.microsoft.com/download/dotnet/9.0
```

**Why this is VITAL:**
- The `global.json` file pins the SDK to version 9.0.304
- Cross-platform build targets require .NET 9 features
- Build will fail immediately without proper SDK version
- All subsequent development attempts will be futile without this

### 2. Non-Shallow Clone Requirement - MANDATORY

You MUST work with a full repository clone, not a shallow one. This is essential for the build system to function properly.

**If working with a shallow clone, fix it immediately:**

```bash
# Convert shallow clone to full clone
git fetch --unshallow

# Verify you have full history
git log --oneline | wc -l  # Should show more than just recent commits
```

**Why this is VITAL:**
- Cross-platform build compatibility depends on full git history
- Version generation and build scripts rely on commit history
- Shallow clones cause build failures and inconsistent behavior
- The build system has been significantly improved for cross-platform compatibility

## Development Workflow

### Initial Setup (MANDATORY FIRST STEPS)

1. **Verify .NET 9 SDK installation**
2. **Ensure non-shallow repository clone**
3. **Only then proceed with development tasks**

### Build and Test

```bash
# Build the main solution (from repository root)
dotnet restore src/ReactiveUI.Uno.sln
dotnet build src/ReactiveUI.Uno.sln

# Run tests
dotnet test src/ReactiveUI.Uno.sln
```

### Platform-Specific Notes

The project supports multiple target frameworks:
- `net9.0` (cross-platform)
- `net9.0-desktop` (desktop targets)
- `net9.0-browserwasm` (WebAssembly)
- `net9.0-windows10.0.19041.0` (Windows desktop - Windows hosts only)
- `net9.0-android` (Android - all platforms)
- `net9.0-ios` (iOS - macOS and Windows hosts only)

Target frameworks are automatically selected based on the host platform for optimal build performance.

## Validation and Quality Assurance

### Code Style and Analysis Enforcement
- **EditorConfig Compliance**: Repository uses a comprehensive `.editorconfig` with detailed rules for C# formatting, naming conventions, and code analysis.
- **StyleCop Analyzers**: Enforces consistent C# code style with `stylecop.analyzers`.
- **Roslynator Analyzers**: Additional code quality rules with `Roslynator.Analyzers`.
- **Analysis Level**: Set to `latest` with enhanced .NET analyzers enabled.
- **CRITICAL**: All code must comply with ReactiveUI contribution guidelines: [https://www.reactiveui.net/contribute/index.html](https://www.reactiveui.net/contribute/index.html).

## C# Style Guide
**General Rule**: Follow "Visual Studio defaults" with the following specific requirements:

### Brace Style
- **Allman style braces**: Each brace begins on a new line.
- **Single line statement blocks**: Can go without braces but must be properly indented on its own line and not nested in other statement blocks that use braces.
- **Exception**: A `using` statement is permitted to be nested within another `using` statement by starting on the following line at the same indentation level, even if the nested `using` contains a controlled block.

### Indentation and Spacing
- **Indentation**: Four spaces (no tabs).
- **Spurious free spaces**: Avoid, e.g., `if (someVar == 0)...` where dots mark spurious spaces.
- **Empty lines**: Avoid more than one empty line at any time between members of a type.
- **Labels**: Indent one less than the current indentation (for `goto` statements).

### Field and Property Naming
- **Internal and private fields**: Use `_camelCase` prefix with `readonly` where possible.
- **Static fields**: `readonly` should come after `static` (e.g., `static readonly` not `readonly static`).
- **Public fields**: Use PascalCasing with no prefix (use sparingly).
- **Constants**: Use PascalCasing for all constant local variables and fields (except interop code, where names and values must match the interop code exactly).
- **Fields placement**: Specify fields at the top within type declarations.

### Visibility and Modifiers
- **Always specify visibility**: Even if it's the default (e.g., `private string _foo` not `string _foo`).
- **Visibility first**: Should be the first modifier (e.g., `public abstract` not `abstract public`).
- **Modifier order**: `public`, `private`, `protected`, `internal`, `static`, `extern`, `new`, `virtual`, `abstract`, `sealed`, `override`, `readonly`, `unsafe`, `volatile`, `async`.

### Namespace and Using Statements
- **Namespace imports**: At the top of the file, outside of `namespace` declarations.
- **Sorting**: System namespaces alphabetically first, then third-party namespaces alphabetically.
- **Global using directives**: Use where appropriate to reduce repetition across files.
- **Placement**: Use `using` directives outside `namespace` declarations.

### Type Usage and Variables
- **Language keywords**: Use instead of BCL types (e.g., `int`, `string`, `float` instead of `Int32`, `String`, `Single`) for type references and method calls (e.g., `int.Parse` instead of `Int32.Parse`).
- **var usage**: Encouraged for large return types or refactoring scenarios; use full type names for clarity when needed.
- **this. avoidance**: Avoid `this.` unless absolutely necessary.
- **nameof(...)**: Use instead of string literals whenever possible and relevant.

### Code Patterns and Features
- **Method groups**: Use where appropriate.
- **Pattern matching**: Use C# 7+ pattern matching, including recursive, tuple, positional, type, relational, and list patterns for expressive conditional logic.
- **Inline out variables**: Use C# 7 inline variable feature with `out` parameters.
- **Non-ASCII characters**: Use Unicode escape sequences (`\uXXXX`) instead of literal characters to avoid garbling by tools or editors.
- **Modern C# features (C# 8–12)**:
  - Enable nullable reference types to reduce null-related errors.
  - Use ranges (`..`) and indices (`^`) for concise collection slicing.
  - Employ `using` declarations for automatic resource disposal.
  - Declare static local functions to avoid state capture.
  - Prefer switch expressions over statements for concise control flow.
  - Use records and record structs for data-centric types with value semantics.
  - Apply init-only setters for immutable properties.
  - Utilize target-typed `new` expressions to reduce verbosity.
  - Declare static anonymous functions or lambdas to prevent state capture.
  - Use file-scoped namespace declarations for concise syntax.
  - Apply `with` expressions for nondestructive mutation.
  - Use raw string literals (`"""`) for multi-line or complex strings.
  - Mark required members with the `required` modifier.
  - Use primary constructors to centralize initialization logic.
  - Employ collection expressions (`[...]`) for concise array/list/span initialization.
  - Add default parameters to lambda expressions to reduce overloads.

### Documentation Requirements
- **XML comments**: All publicly exposed methods and properties must have .NET XML comments, including protected methods of public classes.
- **Documentation culture**: Use `en-US` as specified in `src/stylecop.json`.

### File Style Precedence
- **Existing style**: If a file differs from these guidelines (e.g., private members named `m_member` instead of `_member`), the existing style in that file takes precedence.
- **Consistency**: Maintain consistency within individual files.

### Notes
- **EditorConfig**: The `.editorconfig` at the root of the ReactiveUI repository enforces formatting and analysis rules, replacing the previous `analyzers.ruleset`. Update `.editorconfig` as needed to support modern C# features, such as nullable reference types.
- **Example Updates**: The example incorporates modern C# practices like file-scoped namespaces and nullable reference types. Refer to Microsoft documentation for further integration of C# 8–12 features.

### Code Formatting (Fast - Always Run)
- **ALWAYS** run formatting before committing:
  ```bash
  cd src
  dotnet format whitespace --verify-no-changes
  dotnet format style --verify-no-changes
  ```
  Time: **2-5 seconds per command**.

### Code Analysis Validation
- **Run analyzers** to check StyleCop and code quality compliance:
  ```bash
  cd src
  dotnet build --configuration Release --verbosity normal
  ```
  This runs all analyzers (StyleCop SA*, Roslynator RCS*, .NET CA*) and treats warnings as errors.
- **Analyzer Configuration**:
  - StyleCop settings in `src/stylecop.json`
  - EditorConfig rules in `.editorconfig` (root level)
  - Analyzer packages in `src/Directory.Build.props`
  - All code must follow the **ReactiveUI C# Style Guide** detailed above

## Common Issues and Solutions

### "A compatible .NET SDK was not found"
- **Solution**: Install .NET 9 SDK (version 9.0.304 or later)
- **Do NOT**: Modify global.json to use an older SDK version

### Build failures with version/history errors
- **Solution**: Ensure you have a full clone (`git fetch --unshallow`)
- **Do NOT**: Attempt workarounds with shallow clones

### Cross-platform build issues
- **Solution**: Verify both .NET 9 SDK and full clone requirements are met
- **Note**: Recent improvements require these prerequisites for proper functionality

Remember: These requirements exist because significant work has been done to improve cross-platform build compatibility. Respecting these prerequisites ensures you benefit from these improvements rather than fighting against them.
