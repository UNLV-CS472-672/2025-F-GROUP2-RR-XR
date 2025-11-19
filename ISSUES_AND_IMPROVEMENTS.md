# Rebel Reality - Issues and Improvements

This document catalogs code quality issues, bugs, technical debt, and suggested improvements found during comprehensive code review and documentation.

**Last Updated**: November 2025
**Review Date**: November 2025

---

## Table of Contents

1. [Critical Issues](#critical-issues)
2. [High Priority](#high-priority)
3. [Medium Priority](#medium-priority)
4. [Low Priority / Nice to Have](#low-priority--nice-to-have)
5. [Code Quality Issues](#code-quality-issues)
6. [Architecture Improvements](#architecture-improvements)
7. [Performance Optimizations](#performance-optimizations)
8. [Documentation Gaps](#documentation-gaps)

---

## Critical Issues

### 1. Potential Logic Bug in signLabelLook.cs
**File**: `Assets/Scripts/signLabelLook.cs:46`
**Issue**: Bitwise AND operator used instead of logical AND
```csharp
// Line 46 - POTENTIAL BUG
if (player != null & other.transform == player)  // Using & instead of &&
```

**Impact**:
- Both conditions are always evaluated (no short-circuit)
- If `player` is null, `other.transform == player` still executes
- Could cause NullReferenceException in edge cases

**Fix**:
```csharp
if (player != null && other.transform == player)  // Correct logical AND
```

**Priority**: CRITICAL - Fix immediately
**Estimated Effort**: 1 minute

---

### 2. ROI Coordinate System Mismatch (ARCameraCaptureFrame.cs)
**File**: `Assets/scripts/ARCameraCaptureFrame.cs:48-56`
**Issue**: Unity to iOS Vision coordinate system conversion needs calibration

```csharp
// Lines 48-50 - KNOWN ISSUE (commented in code)
//ISSUES: ACKNOLWEDGE HOW ROI WORKS ON APPLE AND UNITY, CONFIGURE IT TO UNITY -> APPLE
//        TO MAKE IT WORK.
Rect normalizedROI = new Rect(0.3f, 0.3f, 0.4f, 0.2f);
```

**Impact**:
- OCR region may not align with visual overlay
- Text recognition may miss target area
- User experience degraded (confusing UI)

**Fix Required**:
1. Test ROI coordinates on actual iOS device
2. Determine coordinate system transformation
3. Adjust `normalizedROI` or add conversion function
4. Update `updateRoiOverlay()` to match Vision framework expectations

**Priority**: HIGH
**Estimated Effort**: 2-4 hours (requires iOS device testing)

---

## High Priority

### 3. Hardcoded Room Database
**File**: `Assets/ClassSearchFunction.cs:34-49`
**Issue**: Room data hardcoded in C# instead of external data file

**Current**:
```csharp
items = new List<RoomItem>
{
    new RoomItem { roomName = "100 COLLABORATORIUM", buildingName = "AEB" },
    new RoomItem { roomName = "130 FLEXATORIUM", buildingName = "AEB" },
    // ... more rooms
};
```

**Problems**:
- Requires code recompilation to add/update rooms
- Not scalable for multiple buildings
- No way for non-programmers to update data

**Recommended Fix**:
Create JSON configuration file:

```json
// Assets/Resources/RoomDatabase.json
{
  "rooms": [
    {
      "id": "aeb_100",
      "roomName": "100 COLLABORATORIUM",
      "buildingName": "AEB",
      "floor": 1,
      "coordinates": {"x": 10.5, "y": 0, "z": 20.3}
    },
    // ...
  ]
}
```

Load at runtime:
```csharp
void Awake()
{
    TextAsset jsonFile = Resources.Load<TextAsset>("RoomDatabase");
    RoomDatabase db = JsonUtility.FromJson<RoomDatabase>(jsonFile.text);
    items = db.rooms;
}
```

**Benefits**:
- Easy updates without recompilation
- Can be managed by non-developers
- Supports hot-reloading in editor
- Scalable to hundreds of rooms

**Priority**: HIGH
**Estimated Effort**: 2-3 hours

---

### 4. Stub/Placeholder Scripts Should Be Implemented or Removed
**Files**:
- `Assets/ChangeSceneDark.cs` - Completely empty
- `Assets/Scripts/debugCamera.cs` - Empty stub
- `Assets/scripts/OCRDisplay.cs` - All code commented out

**Issue**: Dead code increases maintenance burden and confusion

**Recommendations**:

#### Option A: Implement
- **ChangeSceneDark.cs**: Implement scene transition or dark mode toggle
- **debugCamera.cs**: Add AR debugging features (tracking quality, feature points)
- **OCRDisplay.cs**: Move functionality to ARCameraCaptureFrame or remove

#### Option B: Remove
If not needed for v1.0, delete these files to reduce clutter

**Priority**: HIGH (decide what to do with these)
**Estimated Effort**:
- Remove: 5 minutes
- Implement: 4-8 hours each

---

### 5. Missing Error Handling in Vision Bridge
**File**: `Assets/scripts/visionBridge.cs:20-27`
**Issue**: No error handling for null/empty image data

**Current**:
```csharp
if (imageData == null || imageData.Length == 0)
{
    Debug.LogWarning("RecognizeARFrame called with empty image data.");
    return;  // Silent failure
}
```

**Problems**:
- User has no feedback when OCR fails
- No retry mechanism
- Silent failures are hard to debug

**Recommended Fix**:
```csharp
public static void RecognizeARFrame(byte[] imageData, ...)
{
    if (imageData == null || imageData.Length == 0)
    {
        Debug.LogError("RecognizeARFrame: Invalid image data");
        OnOCRError?.Invoke("Camera capture failed. Please try again.");
        return;
    }

    try
    {
        recognizeTextFromBytes(imageData, width, height, roiX, roiY, roiW, roiH);
    }
    catch (Exception e)
    {
        Debug.LogError($"Vision Framework error: {e.Message}");
        OnOCRError?.Invoke("Text recognition failed.");
    }
}
```

**Priority**: HIGH
**Estimated Effort**: 1 hour

---

## Medium Priority

### 6. Label Font Loading Should Be Cached
**File**: `Assets/Scripts/createLabel.cs:46`
**Issue**: Font loaded from Resources every time a label is created

**Current**:
```csharp
private void labelCreate(Transform target)
{
    // ...
    TMP_FontAsset monsterrateFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/montserratFont");
    // ...
}
```

**Problem**:
- `Resources.Load` is slow (disk I/O)
- Called multiple times (every label creation)
- Unnecessary performance overhead

**Fix**:
```csharp
// Cache at class level
private static TMP_FontAsset _cachedFont;

private void labelCreate(Transform target)
{
    if (_cachedFont == null)
    {
        _cachedFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/montserratFont");
        if (_cachedFont == null)
            Debug.LogError("Failed to load Montserrat font!");
    }
    tmp.font = _cachedFont;
}
```

**Benefits**:
- Font loaded once instead of N times
- Faster label creation
- Better performance

**Priority**: MEDIUM
**Estimated Effort**: 15 minutes

---

### 7. Inconsistent Naming Conventions
**Issue**: Mix of camelCase and PascalCase for private methods

**Examples**:
- `XRToggle.cs`: `toggleARMode()` and `setARMode()` are camelCase (good)
- `ClassSearchFunction.cs`: Some methods PascalCase, some camelCase

**Impact**:
- Inconsistency makes code harder to read
- Violates C# conventions (private methods should be camelCase)

**Recommendation**:
Standardize to:
- **Public methods**: PascalCase (`SelectByIndex`)
- **Private methods**: camelCase (`updateResultsList`)
- **Fields**: camelCase or `_camelCase` with underscore

**Priority**: MEDIUM
**Estimated Effort**: 1-2 hours (refactoring)

---

### 8. Missing Input Validation in ClassSearchFunction
**File**: `Assets/ClassSearchFunction.cs:112-128`
**Issue**: No bounds checking or validation for indices

**Current**:
```csharp
public void SelectByIndex(int index)
{
    if (index < 0 || index >= items.Count)
    {
        return;  // Silent failure
    }
    // ...
}
```

**Problem**:
- Silent failures hide bugs
- No user feedback

**Fix**:
```csharp
public void SelectByIndex(int index)
{
    if (index < 0 || index >= items.Count)
    {
        Debug.LogWarning($"SelectByIndex: Index {index} out of range [0, {items.Count - 1}]");
        Show("Error: Invalid room selection");
        return;
    }
    // ...
}
```

**Priority**: MEDIUM
**Estimated Effort**: 30 minutes

---

### 9. No Unsubscribe from Events
**File**: `Assets/ClassSearchFunction.cs:55`
**Issue**: Event listeners added but never removed

**Current**:
```csharp
void Awake()
{
    searchBox.onValueChanged.AddListener(UpdateResultsList);
    // No corresponding RemoveListener
}
```

**Problem**:
- Memory leaks if component is destroyed and recreated
- Multiple listeners if Awake() called multiple times

**Fix**:
```csharp
void OnDestroy()
{
    if (searchBox != null)
    {
        searchBox.onValueChanged.RemoveListener(UpdateResultsList);
    }
}
```

**Priority**: MEDIUM
**Estimated Effort**: 15 minutes

---

### 10. Commented-Out Code Should Be Removed
**Files**:
- `Assets/Scripts/appearMarkerRad.cs` - Entire implementation commented
- `Assets/Scripts/labelOrbit.cs:31-40` - Alternative implementations commented
- `Assets/scripts/ARCameraCaptureFrame.cs:131-137` - Debug code commented
- `Assets/Scripts/createLabel.cs:47-48` - Font loading test commented

**Issue**:
- Clutters codebase
- Creates confusion
- Should use Git history instead

**Recommendation**:
- **Option A**: Remove commented code (use Git to retrieve if needed)
- **Option B**: Document why code is commented and when it might be needed

**Priority**: MEDIUM
**Estimated Effort**: 30 minutes

---

## Low Priority / Nice to Have

### 11. Object Pooling for Labels
**File**: `Assets/Scripts/createLabel.cs`
**Improvement**: Reuse label GameObjects instead of Instantiate/Destroy

**Current**:
```csharp
void OnTriggerEnter() { labelCreate(); }   // Instantiate
void OnTriggerExit() { labelDestroy(); }   // Destroy
```

**Problems**:
- Instantiate/Destroy causes GC pressure
- Performance spikes when creating/destroying many labels

**Suggested Implementation**:
```csharp
// Simple object pool
private Queue<GameObject> labelPool = new Queue<GameObject>();
private const int POOL_SIZE = 10;

void Start()
{
    // Pre-create pool
    for (int i = 0; i < POOL_SIZE; i++)
    {
        GameObject label = Instantiate(labelPrefab);
        label.SetActive(false);
        labelPool.Enqueue(label);
    }
}

GameObject GetLabel()
{
    if (labelPool.Count > 0)
        return labelPool.Dequeue();
    return Instantiate(labelPrefab);  // Fallback
}

void ReturnLabel(GameObject label)
{
    label.SetActive(false);
    labelPool.Enqueue(label);
}
```

**Benefits**:
- Eliminates GC spikes
- Better frame rate stability
- Improved performance

**Priority**: LOW (optimize later)
**Estimated Effort**: 2-3 hours

---

### 12. Add Search History/Autocomplete
**File**: `Assets/ClassSearchFunction.cs`
**Enhancement**: Remember recent searches and suggest completions

**Implementation Ideas**:
- Store recent searches in PlayerPrefs
- Show "Recent Searches" section
- Fuzzy matching for better results
- Search by room number OR name

**Priority**: LOW (nice-to-have)
**Estimated Effort**: 4-6 hours

---

### 13. Add Unit Tests
**Missing**: Comprehensive unit tests for core logic

**Suggested Tests**:
```csharp
// ClassSearchFunctionTests.cs
[Test]
public void SearchRooms_PartialMatch_ReturnsCorrectResults()
{
    // Test partial string matching
}

[Test]
public void SearchRooms_NoMatch_ReturnsEmptyList()
{
    // Test no results case
}

[Test]
public void SelectByIndex_InvalidIndex_LogsWarning()
{
    // Test error handling
}
```

**Priority**: LOW (but important for long-term maintainability)
**Estimated Effort**: 8-12 hours (comprehensive test suite)

---

## Code Quality Issues

### 14. Magic Numbers Should Be Constants
**Examples**:

```csharp
// createLabel.cs:34
label.transform.localScale = new Vector3(100f, 100f, 100f);

// createLabel.cs:54
tmp.transform.localScale = new Vector3(0.004f, 0.004f, 0.01f);

// signLabelLook.cs:40
Quaternion.Euler(0, 180f, 0)
```

**Fix**:
```csharp
private const float LABEL_SCALE = 100f;
private const float TEXT_SCALE_XY = 0.004f;
private const float TEXT_SCALE_Z = 0.01f;
private const float BILLBOARD_ROTATION_OFFSET = 180f;
```

**Priority**: LOW
**Estimated Effort**: 1 hour

---

### 15. Inconsistent Logging
**Issue**: Mix of Debug.Log, Debug.LogWarning, Debug.LogError

**Recommendation**:
Standardize logging levels:
- **Debug.Log**: Informational (can be disabled in builds)
- **Debug.LogWarning**: Unexpected but recoverable
- **Debug.LogError**: Actual errors that break functionality

Consider creating a logging wrapper:
```csharp
public static class RRLogger
{
    public static void Info(string message) { Debug.Log($"[RR] {message}"); }
    public static void Warn(string message) { Debug.LogWarning($"[RR] {message}"); }
    public static void Error(string message) { Debug.LogError($"[RR] {message}"); }
}
```

**Priority**: LOW
**Estimated Effort**: 2 hours

---

## Architecture Improvements

### 16. Separate Data from Logic (MVC Pattern)
**Current**: ClassSearchFunction mixes data, logic, and UI

**Recommendation**:
```
ClassSearchModel (Data)
    ├─ RoomDatabase
    └─ Search logic

ClassSearchController (Logic)
    ├─ Handle user input
    ├─ Update model
    └─ Notify view

ClassSearchView (UI)
    ├─ Display results
    ├─ Handle button clicks
    └─ Update based on model
```

**Benefits**:
- Better testability
- Easier to swap data sources
- Clearer responsibilities

**Priority**: LOW (architectural improvement)
**Estimated Effort**: 4-6 hours (refactoring)

---

### 17. Implement Pathfinding System
**Current**: Direct navigation to markers only
**Planned**: Dijkstra's algorithm for shortest path

**Status**: Mentioned in feature branches but not implemented

**Priority**: LOW (future feature)
**Estimated Effort**: 16-24 hours (new feature)

---

## Performance Optimizations

### 18. Reduce Update() Calls
**Issue**: Multiple scripts use Update() when events would be better

**Examples**:
- `createLabel.Update()` - Could use events instead
- `XRToggle.Update()` - Currently empty

**Recommendation**:
```csharp
// Instead of polling in Update()
void Update()
{
    if (condition) DoSomething();
}

// Use events
public UnityEvent OnConditionMet;
// Invoke when condition changes
```

**Priority**: LOW
**Estimated Effort**: 2-3 hours

---

### 19. Profile and Optimize AR Frame Processing
**Current**: No profiling data available

**Recommendation**:
1. Use Unity Profiler on iOS device
2. Identify CPU/GPU hotspots
3. Optimize based on data

**Areas to Check**:
- QR code decoding frequency
- Label instantiation performance
- TextMeshPro rendering
- AR frame conversion overhead

**Priority**: LOW (profile before optimizing)
**Estimated Effort**: 4-8 hours

---

## Documentation Gaps

### 20. Missing Documentation
**Needed**:
- [ ] User Manual (how to use the app)
- [ ] iOS Build Guide (step-by-step)
- [ ] Troubleshooting Guide
- [ ] API Documentation (for extending the system)

**Priority**: LOW
**Estimated Effort**: 4-8 hours

---

## Summary Statistics

### Issue Counts by Priority
- **Critical**: 2 issues
- **High**: 5 issues
- **Medium**: 5 issues
- **Low**: 8 improvements

### Estimated Total Effort
- **Critical fixes**: 1-4 hours
- **High priority**: 10-18 hours
- **Medium priority**: 4-6 hours
- **Low priority**: 40-60 hours

### Recommended Action Plan

**Phase 1: Critical Fixes (Do Immediately)**
1. Fix bitwise AND bug (signLabelLook.cs:46)
2. Test and fix ROI coordinate system

**Phase 2: High Priority (Next Sprint)**
1. Move room database to JSON
2. Decide fate of stub scripts
3. Add error handling to vision bridge

**Phase 3: Medium Priority (Future Sprints)**
1. Implement object pooling for labels
2. Add unit tests
3. Clean up commented code

**Phase 4: Low Priority (As Time Permits)**
1. Architecture refactoring
2. Performance optimization
3. Additional features

---

## How to Use This Document

1. **Review Issues**: Read through and prioritize based on your team's goals
2. **Create GitHub Issues**: Convert items into trackable GitHub issues
3. **Assign Ownership**: Assign issues to team members
4. **Track Progress**: Update this document as issues are resolved
5. **Regular Review**: Review quarterly to add new issues

---

## Contributing

Found a new issue? Add it to this document following the template:

```markdown
### N. Issue Title
**File**: `path/to/file.cs:line`
**Issue**: Brief description

**Current**: Code snippet showing the problem

**Problem**: Why it's an issue

**Fix**: Suggested solution with code

**Priority**: CRITICAL | HIGH | MEDIUM | LOW
**Estimated Effort**: Time estimate
```

---

**Last Reviewed**: November 2025
**Next Review**: After v1.0 release
**Maintainers**: Rebel Reality Team
