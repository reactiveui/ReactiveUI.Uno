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