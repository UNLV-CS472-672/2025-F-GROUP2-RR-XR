# Contributing to Rebel Reality

Thank you for your interest in contributing to Rebel Reality! This document provides guidelines and best practices for contributing to the project.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Workflow](#development-workflow)
- [Coding Standards](#coding-standards)
- [Testing Guidelines](#testing-guidelines)
- [Commit Guidelines](#commit-guidelines)
- [Pull Request Process](#pull-request-process)
- [Documentation](#documentation)
- [Unity-Specific Guidelines](#unity-specific-guidelines)
- [AR/XR Best Practices](#arxr-best-practices)

---

## Code of Conduct

### Our Standards

- **Be Respectful**: Treat all contributors with respect and professionalism
- **Be Collaborative**: Work together to solve problems and improve the project
- **Be Constructive**: Provide helpful feedback and be open to receiving it
- **Be Inclusive**: Welcome contributors of all skill levels and backgrounds

### Unacceptable Behavior

- Harassment, discrimination, or offensive comments
- Trolling, insulting, or derogatory remarks
- Publishing others' private information
- Any conduct that would be inappropriate in a professional setting

---

## Getting Started

### Prerequisites

Before contributing, ensure you have:
- Unity 6000.0.57f1 installed
- Git configured with your name and email
- Read the [README.md](README.md) and [ARCHITECTURE.md](ARCHITECTURE.md)
- Familiarity with C# and Unity basics

### Setting Up Your Development Environment

1. **Fork the Repository**
   ```bash
   # Fork via GitHub UI, then clone your fork
   git clone https://github.com/YOUR_USERNAME/2025-F-GROUP2-RR-XR.git
   cd 2025-F-GROUP2-RR-XR
   ```

2. **Add Upstream Remote**
   ```bash
   git remote add upstream https://github.com/UNLV-CS472-672/2025-F-GROUP2-RR-XR.git
   ```

3. **Open in Unity**
   - Open Unity Hub
   - Add project from disk
   - Ensure Unity 6000.0.57f1 is used

4. **Verify Build**
   - Try building for iOS to ensure everything works
   - Run any existing tests

---

## Development Workflow

### Branching Strategy

We use **Git Flow** with the following branch types:

- `main` - Production-ready code
- `dev` - Development branch (current working branch)
- `feature/feature-name` - New features
- `bugfix/bug-description` - Bug fixes
- `hotfix/critical-fix` - Critical production fixes

### Creating a Feature Branch

```bash
# Update dev branch
git checkout dev
git pull upstream dev

# Create feature branch
git checkout -b feature/your-feature-name

# Work on your feature...
git add .
git commit -m "Add feature: description"

# Push to your fork
git push origin feature/your-feature-name
```

### Keeping Your Branch Updated

```bash
# Fetch upstream changes
git fetch upstream

# Rebase on dev
git checkout feature/your-feature-name
git rebase upstream/dev

# Resolve conflicts if any, then:
git push origin feature/your-feature-name --force-with-lease
```

---

## Coding Standards

### C# Coding Conventions

Follow [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)

#### Naming Conventions

```csharp
// Classes: PascalCase
public class ClassSearchFunction { }

// Public methods: PascalCase
public void SelectByIndex(int index) { }

// Private methods: camelCase
private void updateResultsList(string query) { }

// Private fields: camelCase with underscore prefix (optional)
private bool _arModeActive = false;

// Serialized fields: camelCase
[SerializeField] private GameObject userUI;

// Constants: PascalCase or UPPER_CASE
private const float MAX_DISTANCE = 100f;
```

#### Formatting

```csharp
// Use 4 spaces for indentation (not tabs)
public void ExampleMethod()
{
    if (condition)
    {
        DoSomething();
    }
    else
    {
        DoSomethingElse();
    }
}

// Braces on new lines (Allman style)
public class MyClass
{
    public void MyMethod()
    {
        // Method body
    }
}

// Single-line statements still use braces
if (condition)
{
    DoSomething();
}

// Use var only when type is obvious
var items = new List<RoomItem>();  // Good
var result = GetResult();           // Avoid (not obvious)
```

#### Documentation Comments

**All public methods and classes MUST have XML documentation**:

```csharp
/// <summary>
/// Searches for rooms that contain the query string (case-insensitive partial matching).
/// </summary>
/// <param name="query">The search query string</param>
/// <returns>List of indices for rooms that match the query</returns>
private List<int> SearchRooms(string query)
{
    // Implementation
}
```

**Use Tooltips for Unity Inspector fields**:

```csharp
[Header("UI References")]
[SerializeField]
[Tooltip("Input field for user search queries")]
private TMP_InputField searchBox;
```

---

## Testing Guidelines

### Unity Play Mode Tests

Create tests for new features in `Assets/Tests/`:

```csharp
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class ClassSearchFunctionTests
{
    [Test]
    public void SearchRooms_PartialMatch_ReturnsCorrectResults()
    {
        // Arrange
        var searchFunc = new ClassSearchFunction();

        // Act
        var results = searchFunc.SearchRooms("flex");

        // Assert
        Assert.Greater(results.Count, 0);
        Assert.IsTrue(results[0].roomName.Contains("FLEX", StringComparison.OrdinalIgnoreCase));
    }
}
```

### Manual Testing Checklist

Before submitting a PR, test:

- [ ] Feature works in Unity Editor
- [ ] Feature works on iOS device
- [ ] No console errors or warnings
- [ ] UI is responsive and properly positioned
- [ ] Performance is acceptable (60 FPS target)
- [ ] No memory leaks (check Profiler)

---

## Commit Guidelines

### Commit Message Format

Follow [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, no logic change)
- `refactor`: Code refactoring (no feature change)
- `perf`: Performance improvements
- `test`: Adding or updating tests
- `chore`: Build process, dependencies, tooling

**Examples:**

```bash
# Good commit messages
git commit -m "feat(search): add partial query matching for room names"
git commit -m "fix(labels): resolve z-fighting issue with TextMeshPro"
git commit -m "docs(architecture): add sequence diagram for search flow"
git commit -m "perf(qr): throttle QR decoding to 1 FPS for better performance"

# Bad commit messages
git commit -m "fixed stuff"
git commit -m "WIP"
git commit -m "asdf"
```

### Commit Best Practices

1. **Make atomic commits** - One logical change per commit
2. **Write clear messages** - Explain what and why, not how
3. **Commit often** - Small, frequent commits are better than large ones
4. **Test before committing** - Ensure code compiles and runs

---

## Pull Request Process

### Before Submitting

1. **Update from dev**
   ```bash
   git checkout dev
   git pull upstream dev
   git checkout feature/your-feature
   git rebase dev
   ```

2. **Run tests**
   - Unity Play Mode tests
   - Manual testing on device

3. **Update documentation**
   - Update README if feature is user-facing
   - Add XML comments to new public methods
   - Update ARCHITECTURE.md for architectural changes

4. **Self-review**
   - Read through your changes
   - Check for commented-out code
   - Verify no debug logs left in production code

### Creating the Pull Request

1. **Push to your fork**
   ```bash
   git push origin feature/your-feature-name
   ```

2. **Open PR on GitHub**
   - Go to the main repository
   - Click "New Pull Request"
   - Select your branch
   - Fill out the PR template

### PR Template

```markdown
## Description
Brief description of what this PR does

## Type of Change
- [ ] New feature
- [ ] Bug fix
- [ ] Documentation update
- [ ] Performance improvement
- [ ] Code refactoring

## Testing
- [ ] Tested in Unity Editor
- [ ] Tested on iOS device
- [ ] Added/updated unit tests
- [ ] No console errors/warnings

## Screenshots (if applicable)
[Add screenshots/videos of UI changes]

## Checklist
- [ ] Code follows project style guidelines
- [ ] Self-reviewed code
- [ ] Commented code where necessary
- [ ] Updated documentation
- [ ] No breaking changes (or documented)

## Related Issues
Fixes #123
Relates to #456
```

### PR Review Process

1. **Automated Checks**
   - GitHub Actions CI/CD must pass
   - No merge conflicts

2. **Code Review**
   - At least one team member must approve
   - Address all review comments

3. **Final Steps**
   - Squash commits if needed
   - Merge via "Squash and Merge" button
   - Delete feature branch

---

## Documentation

### When to Update Documentation

- **README.md**: When adding user-facing features, dependencies, or setup steps
- **ARCHITECTURE.md**: When changing system design, adding components, or modifying data flow
- **Inline comments**: Always for complex logic or non-obvious code
- **XML comments**: Always for public methods and classes

### Documentation Style

- **Be concise** - Short, clear sentences
- **Be specific** - Provide examples and code snippets
- **Be current** - Update docs when code changes
- **Be helpful** - Explain the "why," not just the "what"

---

## Unity-Specific Guidelines

### Scene Management

- **Don't commit large scene changes** without discussion
- **Use prefabs** for reusable components
- **Test in build**, not just editor (some things work differently)

### Asset Organization

```
Assets/
├── Scripts/           # Core game logic scripts
├── Prefabs/           # Reusable prefabs
├── Scenes/            # Unity scenes
├── Materials/         # Materials and shaders
├── Models/            # 3D models
├── Textures/          # Texture assets
└── Tests/             # Test scripts
```

### Performance Best Practices

1. **Object Pooling**
   - Reuse GameObjects instead of Instantiate/Destroy
   - Especially important for frequently created objects (labels)

2. **Avoid Update() When Possible**
   - Use events and coroutines instead
   - Example: `OnTriggerEnter` instead of polling distance

3. **Optimize AR Frame Processing**
   - Process ROI instead of full frame
   - Throttle processing (don't process every frame)

4. **Profiler is Your Friend**
   - Profile before optimizing
   - Focus on CPU and memory hotspots

### Common Pitfalls

1. **Missing Null Checks**
   ```csharp
   // Bad
   myObject.transform.position = newPos;

   // Good
   if (myObject != null)
   {
       myObject.transform.position = newPos;
   }
   ```

2. **Forgetting to Unsubscribe from Events**
   ```csharp
   void OnEnable()
   {
       searchBox.onValueChanged.AddListener(UpdateResults);
   }

   void OnDisable()
   {
       searchBox.onValueChanged.RemoveListener(UpdateResults);
   }
   ```

3. **Hardcoding Values**
   ```csharp
   // Bad
   if (distance < 5.0f) { }

   // Good
   [SerializeField] private float triggerDistance = 5.0f;
   if (distance < triggerDistance) { }
   ```

---

## AR/XR Best Practices

### ARFoundation Guidelines

1. **Check AR Session State**
   ```csharp
   void Start()
   {
       StartCoroutine(WaitForARSession());
   }

   IEnumerator WaitForARSession()
   {
       while (ARSession.state != ARSessionState.SessionTracking)
       {
           yield return null;
       }
       // AR is ready
   }
   ```

2. **Dispose of CPU Images**
   ```csharp
   if (arCamManager.TryAcquireLatestCpuImage(out XRCpuImage image))
   {
       // Process image
       image.Dispose();  // Important!
   }
   ```

3. **Handle AR Session Interruptions**
   - Phone calls
   - App backgrounding
   - Camera permissions denied

### Performance Targets

- **Target**: 60 FPS on iPhone XR or newer
- **Acceptable**: 30 FPS minimum
- **Memory**: < 500MB total usage
- **Battery**: < 20% drain per hour

---

## Questions or Issues?

- **Technical Questions**: Open a GitHub Discussion
- **Bug Reports**: Open a GitHub Issue with the bug template
- **Feature Requests**: Open a GitHub Issue with the feature template
- **Urgent Matters**: Contact team members directly (see contributors.txt)

---

## Attribution

Contributors are automatically added to the project through Git history. Significant contributions may be highlighted in the README.

Thank you for contributing to Rebel Reality!

---

**Last Updated**: November 2025
**Maintainers**: Rebel Reality Development Team
