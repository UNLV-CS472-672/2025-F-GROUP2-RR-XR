# Assets/Scripts - Core AR Navigation Scripts

This directory contains the core C# scripts for the Rebel Reality AR navigation system. These scripts handle XR mode toggling, marker visibility, and dynamic label generation.

## Directory Structure

```
Assets/Scripts/
├── XRToggle.cs           # AR mode toggle and UI management
├── hideMarkers.cs        # Navigation marker visibility control
├── createLabel.cs        # Dynamic 3D label instantiation
├── labelOrbit.cs         # Label rotation behavior
├── signLabelLook.cs      # Camera-facing label behavior
├── appearMarkerRad.cs    # ARCHIVED: Proximity-based marker appearance
├── debugCamera.cs        # STUB: AR camera debugging utilities
└── README.md             # This file
```

---

## Script Descriptions

### Active Scripts

#### XRToggle.cs
**Purpose**: Controls AR mode activation/deactivation
**Attached to**: Main UI Canvas or AR Manager GameObject

**Key Features**:
- Toggles AR camera background on/off
- Switches between user UI and navigation UI
- Manages AR session state
- Supports multiple toggle buttons

**Usage**:
```csharp
// Automatically toggles AR mode when button is clicked
// Set up in Unity Inspector:
// 1. Assign toggle buttons to toggleButtons list
// 2. Assign AR components (arSession, arCameraBackground, etc.)
// 3. Assign UI panels (userUI, navUI)
```

**See**: [XRToggle.cs](XRToggle.cs) for full documentation

---

#### hideMarkers.cs
**Purpose**: Centralized manager for navigation marker visibility
**Attached to**: Navigation System GameObject

**Key Features**:
- Hides all markers by default on scene start
- Shows only the selected destination marker
- Tracks navigation active/inactive state
- Prevents marker visibility conflicts

**Usage**:
```csharp
// Get reference to hideMarkers component
hideMarkers markerManager = GetComponent<hideMarkers>();

// Show a specific marker
markerManager.showMarker(targetTransform, enableNavigation: true);

// Hide all markers
markerManager.hideMarkersVisual();

// Check navigation state
bool isNavigating = markerManager.getnavActive();
```

**See**: [hideMarkers.cs](hideMarkers.cs) for full documentation

---

#### createLabel.cs
**Purpose**: Dynamically creates 3D TextMeshPro labels for room markers
**Attached to**: Each room marker GameObject (as child collider trigger)
**Author**: Alex Yamasaki

**Key Features**:
- Spawns labels when player enters proximity trigger
- Destroys labels when player exits proximity
- Configures TextMeshPro with custom font and styling
- Adds orbital and camera-facing behaviors automatically

**Label Components**:
1. **Sign Mesh**: 3D visual background for text
2. **TextMeshPro**: Room name text (Montserrat font)
3. **labelOrbit**: Rotation behavior (added automatically)
4. **signLabelLook**: Camera-facing behavior (added automatically)

**Configuration**:
```csharp
[SerializeField] public GameObject parentObject;    // The room marker
[SerializeField] public GameObject labelPrefab;     // Label prefab template
public float labelHeightOffset = 0.3f;              // Height above marker
```

**Usage Flow**:
```
Player Approaches Marker
         │
         ▼
OnTriggerEnter (Collider trigger)
         │
         ▼
labelCreate(parentObject.transform)
         │
         ├─→ Instantiate label prefab
         ├─→ Create TextMeshPro text
         ├─→ Add labelOrbit component
         └─→ Add signLabelLook component
```

**See**: [createLabel.cs](createLabel.cs) for full documentation

---

#### labelOrbit.cs
**Purpose**: Makes labels rotate/spin around markers (far behavior)
**Attached to**: Dynamically added to labels by createLabel.cs

**Key Features**:
- Continuous rotation around Y-axis
- Frame-rate independent (uses Time.deltaTime)
- Defers to signLabelLook when player is nearby
- Configurable speed and radius

**Configuration**:
```csharp
public Transform center;     // The marker to orbit around
public float radius = 1f;    // Orbital radius
public float speed = 30f;    // Rotation speed (degrees/second)
```

**Behavior**:
- **Far**: Label spins around marker (orbital rotation)
- **Near**: Stops orbiting, lets signLabelLook take over

**See**: [labelOrbit.cs](labelOrbit.cs) for full documentation

---

#### signLabelLook.cs
**Purpose**: Makes labels face the camera when player is nearby (near behavior)
**Attached to**: Dynamically added to labels by createLabel.cs

**Key Features**:
- Billboard effect (label always faces player)
- Proximity detection using sphere collider
- Smooth rotation using Quaternion.Slerp
- Y-axis only rotation (prevents tilting)

**Configuration**:
```csharp
public Transform player;     // Main camera (auto-assigned)
public float radius = 2f;    // Detection radius
```

**Behavior Flow**:
```
Player Enters Radius (OnTriggerEnter)
         │
         ▼
playerIsClose = true
         │
         ▼
Update() → Rotate to face camera
         │
         ▼
Player Exits Radius (OnTriggerExit)
         │
         ▼
playerIsClose = false → Stop facing camera
```

**Integration**:
- **labelOrbit** checks `signLabelLook.getPlayerIsClose()`
- If close: labelOrbit stops, signLabelLook takes over
- If far: labelOrbit resumes spinning

**See**: [signLabelLook.cs](signLabelLook.cs) for full documentation

---

### Archived/Stub Scripts

#### appearMarkerRad.cs (ARCHIVED)
**Status**: Inactive - All code commented out
**Original Purpose**: Show markers based on player proximity
**Author**: Alex Yamasaki

**Why Archived**:
- Design decision: Proximity-based marker appearance causes clutter
- Better UX: Only show markers when user explicitly selects destination
- Kept for reference and potential future prototyping

**See**: [appearMarkerRad.cs](appearMarkerRad.cs) for original design notes

---

#### debugCamera.cs (STUB)
**Status**: Placeholder - Not implemented
**Purpose**: AR camera debugging utilities

**Potential Features**:
- Display AR tracking quality
- Show detected feature points
- Log camera intrinsics
- Visualize plane detection
- Debug frame capture

**Implementation Status**: Stub class with empty Start/Update methods

**See**: [debugCamera.cs](debugCamera.cs) for TODO notes

---

## Script Dependencies

### Unity Packages Required
- **UnityEngine** - Core Unity functionality
- **UnityEngine.UI** - UI components (Button, etc.)
- **UnityEngine.XR.ARFoundation** - AR session management
- **TMPro** - TextMeshPro text rendering

### Internal Dependencies
```
XRToggle.cs
  ↓ (no dependencies on other scripts)

hideMarkers.cs
  ↓ (no dependencies on other scripts)

createLabel.cs
  ↓ uses hideMarkers.getnavActive()
  ↓ creates labelOrbit
  ↓ creates signLabelLook

labelOrbit.cs
  ↓ checks signLabelLook.getPlayerIsClose()

signLabelLook.cs
  ↓ (no dependencies on other scripts)
```

---

## Common Workflows

### Workflow 1: Toggle AR Mode
```csharp
// User clicks AR toggle button
XRToggle.toggleARMode()
    ↓
XRToggle.setARMode(true)
    ↓
Enable: arCameraBackground
Disable: userUI
Enable: navUI
```

### Workflow 2: Select Navigation Destination
```csharp
// User selects room from search results
ClassSearchFunction.SelectByIndex(index)
    ↓
ClassSearchFunction.OnDestinationSelected.Invoke(roomName)
    ↓
hideMarkers.showMarker(targetMarker, navActive: true)
    ↓
hideMarkers.hideMarkersVisual()  // Hide all first
hideMarkers.toggleNavActive()     // Set nav state to active
Show target marker children       // Make destination visible
```

### Workflow 3: Proximity-Based Label Spawn
```csharp
// Player walks near marker
OnTriggerEnter (createLabel's trigger collider)
    ↓
Check: !hideMarkers.getnavActive()
    ↓ (only spawn if NOT in navigation mode)
createLabel.labelCreate(parentObject.transform)
    ↓
Instantiate label prefab
Create TextMeshPro text component
Add labelOrbit component
Add signLabelLook component
    ↓
Label appears and starts spinning
```

### Workflow 4: Label Behavior Switching
```csharp
// Every frame
labelOrbit.Update()
    ↓
Check: signLabelLook.getPlayerIsClose()
    ↓
If CLOSE:
    Stop orbiting
    signLabelLook.Update() rotates label to face camera
If FAR:
    Continue orbital rotation
```

---

## Performance Considerations

### Label Lifecycle
**Optimization**: Labels created/destroyed dynamically
- **Why**: Keeping 50+ labels active = low FPS
- **Solution**: Only create when player is nearby
- **Impact**: Maintains 60 FPS with 3-5 active labels

### Update() Optimization
**Best Practice**: Minimize Update() calls
- **labelOrbit.Update()**: Early return if player is close (defers to signLabelLook)
- **signLabelLook.Update()**: Only runs rotation if playerIsClose = true
- **XRToggle.Update()**: Currently empty (reserved for future)

### Collider Usage
**Trigger Colliders**: Used for proximity detection
- **createLabel**: Box/Sphere collider on marker (isTrigger = true)
- **signLabelLook**: Sphere collider on label (isTrigger = true)
- **Optimization**: Use sphere colliders (faster than box)

---

## Debugging Tips

### Common Issues

#### Labels Not Appearing
1. Check `createLabel.parentObject` is assigned in Inspector
2. Verify `labelPrefab` is assigned
3. Ensure collider on marker has `isTrigger = true`
4. Check Navigation mode is OFF (labels hidden when navActive = true)

#### AR Toggle Not Working
1. Verify `arCameraBackground` is assigned
2. Check `userUI` and `navUI` references are correct
3. Ensure toggle buttons are in `toggleButtons` list
4. Check AR session state (should be SessionTracking)

#### Labels Not Facing Camera
1. Verify Main Camera has tag "MainCamera"
2. Check `signLabelLook.player` is assigned (auto-assigns Camera.main)
3. Ensure sphere collider radius is appropriate
4. Check collider is on correct layer (player can trigger it)

### Debug Logs
Key scripts include debug logging (can enable for troubleshooting):
```csharp
// XRToggle.cs
Debug.Log($"AR Mode: {(enable ? "ENABLED" : "DISABLED")}");

// hideMarkers.cs
Debug.Log($"Navigation mode: {(navActive ? "ACTIVE" : "INACTIVE")}");
Debug.Log($"Marker shown: {target.name}");
```

---

## Future Enhancements

### Planned Features
1. **Object Pooling for Labels** - Reuse label GameObjects instead of Instantiate/Destroy
2. **LOD System** - Different label detail levels based on distance
3. **Label Categories** - Different visual styles for rooms, bathrooms, exits
4. **Animated Transitions** - Smooth fade in/out for label appearance
5. **Accessibility** - Audio labels for visually impaired users

### Potential Improvements
- Move label creation to a factory pattern
- Separate label behavior logic from creation logic
- Add label caching for frequently visited areas
- Implement label priority system (important rooms always shown)

---

## Testing

### Manual Testing Checklist
- [ ] AR toggle button switches modes correctly
- [ ] Labels appear when approaching markers
- [ ] Labels disappear when leaving markers
- [ ] Labels spin when player is far
- [ ] Labels face camera when player is near
- [ ] Navigation mode hides labels correctly
- [ ] No console errors or warnings
- [ ] Performance stays above 30 FPS

### Unit Testing
See `Assets/Tests/` for unit tests (if available)

---

## Contributing

When modifying these scripts:
1. Update XML documentation comments
2. Maintain existing functionality
3. Test on iOS device (not just editor)
4. Profile performance changes
5. Update this README if adding new scripts

See [CONTRIBUTING.md](../../CONTRIBUTING.md) for detailed guidelines.

---

## Questions?

- **Script-specific questions**: Check inline XML documentation in each file
- **Architecture questions**: See [ARCHITECTURE.md](../../ARCHITECTURE.md)
- **General questions**: Open a GitHub Discussion

---

**Last Updated**: November 2025
**Maintainers**: Rebel Reality Team
