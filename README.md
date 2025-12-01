# Rebel Reality (RR)

[![Unity Version](https://img.shields.io/badge/Unity-6000.0.57f1-blue)](https://unity.com/)
[![iOS](https://img.shields.io/badge/Platform-iOS-lightgrey)](https://www.apple.com/ios/)
[![ARKit](https://img.shields.io/badge/ARKit-6.0.6-orange)](https://developer.apple.com/arkit/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

**Rebel Reality** is an innovative Extended Reality (XR) indoor navigation system designed for the University of Nevada, Las Vegas (UNLV) campus. Using augmented reality (AR) technology, the application provides real-time wayfinding and room location services within campus buildings.

## Table of Contents

- [Features](#features)
- [Technologies](#technologies)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Project Structure](#project-structure)
- [Architecture](#architecture)
- [Usage](#usage)
- [Build Instructions](#build-instructions)
- [Testing](#testing)
- [Contributing](#contributing)
- [Team](#team)
- [License](#license)

---

## Features

### Core Navigation Features
- **Real-time AR Navigation**: Navigate indoor spaces using AR markers and visual cues
- **Smart Room Search**: Search for classrooms and rooms with partial query matching
- **Interactive Labels**: Dynamic room labels that orbit around markers and face the user
- **QR Code Integration**: Scan QR codes for quick location identification
- **OCR Text Recognition**: iOS native OCR for reading room signs and labels
- **Multi-Building Support**: Currently supports AEB (Applied Engineering Building), TBE-A, and TBE-B

### AR/XR Features
- **Toggle AR Mode**: Switch between standard UI and AR camera view
- **Marker Visibility Management**: Show/hide navigation markers based on context
- **3D Building Models**: High-fidelity GLB models of campus buildings
- **Proximity-Based Interactions**: Labels and markers appear based on user location
- **Visual Localization**: Persistent AR anchors using Immersal SDK

### User Interface
- **Clean Search Interface**: TextMeshPro-powered search with live results
- **Result Filtering**: Displays matching rooms with building information
- **Minimalist Design**: Clean and modern UI components
- **Responsive Feedback**: Real-time status messages and visual feedback

---

## Technologies

### Core Engine & Frameworks
- **Unity 6000.0.57f1** - Game engine and development platform
- **C# (.NET)** - Primary programming language
- **ARFoundation** - Cross-platform AR framework
- **ARKit 6.0.6** - iOS-specific AR capabilities

### AR & Computer Vision
- **Immersal SDK v2.1.1** - Visual localization and persistent mapping
- **ZXing.NET** - QR code and barcode scanning
- **iOS Vision Framework** - Native OCR text recognition (via P/Invoke)

### UI & Graphics
- **TextMesh Pro** - Advanced text rendering and typography
- **Unity UGUI v2.0.0** - User interface system
- **Unity glTF Runtime v6.14.0** - 3D model loading (GLB format)

### Development Tools
- **Visual Studio** - IDE with ManagedGame workload
- **GitHub Actions** - CI/CD automation for iOS builds
- **Unity Input System 1.14.2** - Modern input handling


---

## Prerequisites

### For Unity Development
- **Unity Hub** (latest version)
- **Unity 6000.0.57f1** with the following modules:
  - iOS Build Support
  - Visual Studio for Mac (macOS)
  - Visual Studio (Windows)
- **macOS** (for iOS builds) with Xcode 14.0+
- **iOS Device** with ARKit support (iPhone 6S or later recommended)
- **Apple Developer Account** (for device deployment)

### System Requirements
- **OS**: macOS 12.0+ (for iOS builds) or Windows 10+ (for Unity Editor)
- **RAM**: 16GB minimum, 32GB recommended
- **Storage**: 20GB free space for Unity and project assets
- **GPU**: Metal-compatible (macOS) or DirectX 11+ (Windows)

---

## Installation

### Clone the Repository
```bash
git clone https://github.com/UNLV-CS472-672/2025-F-GROUP2-RR-XR.git
cd 2025-F-GROUP2-RR-XR
```

### Open in Unity
1. Open **Unity Hub**
2. Click **"Add"** → **"Add project from disk"**
3. Navigate to the cloned repository folder
4. Select the project folder (Unity will detect it automatically)
5. Ensure Unity version **6000.0.57f1** is installed
6. Click on the project to open it

### Configure iOS Build Settings
1. In Unity, go to **File** → **Build Settings**
2. Select **iOS** platform
3. Click **"Switch Platform"** if not already on iOS
4. Click **"Player Settings"**
5. Set the following:
   - **Company Name**: Your organization
   - **Product Name**: Rebel Reality
   - **Bundle Identifier**: `com.yourcompany.rebelreality`
   - **Minimum iOS Version**: 14.0 or higher
   - **Camera Usage Description**: "AR navigation requires camera access"
   - **Location Usage Description**: "Navigation requires location access"

### Install Dependencies
Dependencies are managed via Unity Package Manager and should auto-resolve on project load. If issues occur:
1. Go to **Window** → **Package Manager**
2. Verify the following packages are installed:
   - AR Foundation
   - ARKit XR Plugin
   - TextMeshPro
   - Unity Input System
   - glTF Runtime


## Project Structure

```
2025-F-GROUP2-RR-XR/
├── Assets/
│   ├── Scripts/                    # Core C# scripts (XR, labels, markers)
│   ├── scripts/                    # iOS-specific scripts (QR, OCR, vision)
│   ├── ClassSearchFunction.cs      # Room search and selection logic
│   ├── ChangeSceneDark.cs          # Scene management (stub)
│   ├── Scenes/                     # Unity scene files
│   │   ├── RebelReality.unity      # Main application scene
│   │   └── AR Field*.unity         # AR test scenes
│   ├── Prefabs/                    # Reusable UI components
│   │   └── SearchUI.prefab         # Search interface prefab
│   ├── 3D Map Models/              # GLB building models (14+ models)
│   ├── Models/                     # 3D objects (signs, labels)
│   ├── Map Data/                   # JSON metadata for Immersal maps
│   ├── Images/                     # UI textures and graphics
│   ├── Fonts/                      # Typography resources
│   ├── UI/                         # UI prefabs
│   ├── XR/                         # XR configuration
│   ├── Settings/                   # Project settings
│   ├── Plugins/                    # Native iOS plugins
│   ├── TextMesh Pro/               # TMP library
│   ├── Unity-UI-Rounded-Corners/   # UI styling
│   └── GUIPack-Clean&Minimalist/   # UI component kit
├── ProjectSettings/                # Unity engine configuration
├── Packages/                       # Package manifest and dependencies
├── Poster/                         # Project promotional materials
├── contributors.txt                # Team member information
└── README.md                       # This file
```

---

## Architecture

### High-Level Overview
Rebel Reality follows a modular architecture with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────┐
│                      Unity UI Layer                     │
│  (Search Interface, Result Display, AR Controls)        │
└───────────────────┬─────────────────────────────────────┘
                    │
┌───────────────────▼─────────────────────────────────────┐
│                 Application Logic Layer                 │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐   │
│  │ Search       │  │ Navigation   │  │ XR Control   │   │
│  │ Management   │  │ Management   │  │ Management   │   │
│  └──────────────┘  └──────────────┘  └──────────────┘   │
└───────────────────┬─────────────────────────────────────┘
                    │
┌───────────────────▼─────────────────────────────────────┐
│                  AR/Vision Layer                        │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐   │
│  │ ARFoundation │  │ QR Scanner   │  │ OCR Bridge   │   │
│  │ (ARKit)      │  │ (ZXing)      │  │ (iOS Native) │   │
│  └──────────────┘  └──────────────┘  └──────────────┘   │
└───────────────────┬─────────────────────────────────────┘
                    │
┌───────────────────▼─────────────────────────────────────┐
│                   Hardware Layer                        │
│         (iOS Device Camera, Sensors, GPU)               │
└─────────────────────────────────────────────────────────┘
```

### Key Components

#### 1. Search System (`ClassSearchFunction.cs`)
- Manages room database and search queries
- Implements partial string matching (case-insensitive)
- Creates dynamic result buttons
- Triggers navigation on room selection

#### 2. XR Toggle System (`XRToggle.cs`)
- Controls AR mode activation/deactivation
- Manages camera background visibility
- Switches between navigation UI and standard UI

#### 3. Label System
- **`createLabel.cs`**: Dynamically instantiates room labels
- **`labelOrbit.cs`**: Rotates labels around markers
- **`signLabelLook.cs`**: Makes labels face the camera when user is nearby

#### 4. Marker System (`hideMarkers.cs`)
- Controls marker visibility
- Manages navigation state
- Shows/hides waypoint markers

#### 5. Vision Integration
- **`ARQRCodeScanner.cs`**: QR code detection using ZXing
- **`ARCameraCaptureFrame.cs`**: Frame capture for OCR processing
- **`visionBridge.cs`**: P/Invoke bridge to iOS Vision Framework

#### 6. Navigation Flow
```
User Input (Search) → ClassSearchFunction → Room Selection
                                               ↓
                               hideMarkers.showMarker()
                                               ↓
                              createLabel (on proximity)
                                               ↓
                           AR Visualization with Labels
```

For detailed architecture documentation, see [ARCHITECTURE.md](ARCHITECTURE.md).

---

## Usage

### Basic Navigation Workflow

1. **Launch the Application**
   - Open the app on your iOS device
   - Grant camera and location permissions

2. **Search for a Room**
   - Tap the search box
   - Enter a room name or number (e.g., "130" or "Flexatorium")
   - Results appear in real-time as you type

3. **Select a Destination**
   - Tap on the desired room from the search results
   - The navigation system activates

4. **Enable AR Mode**
   - Tap the AR toggle button
   - Point your camera at the environment
   - AR markers and labels will appear

5. **Navigate**
   - Follow the AR markers and labels
   - Labels will face you when you're nearby
   - Markers guide you to your destination

### AR Features

#### QR Code Scanning
- Point your camera at a QR code
- The system automatically detects and processes it
- Results display on-screen

#### OCR Text Recognition (iOS Only)
- Tap the scan button when viewing room signs
- The system extracts text using iOS Vision Framework
- Recognized text appears in the UI

---

## Build Instructions

### iOS Build

#### Via Unity Editor (Manual Build)
1. Open the project in Unity
2. Go to **File** → **Build Settings**
3. Ensure **iOS** is selected
4. Click **"Build"**
5. Choose an output folder
6. Wait for the build to complete
7. Open the generated Xcode project
8. In Xcode:
   - Select your development team
   - Connect your iOS device
   - Click **Run** (Play button)

#### Via GitHub Actions (Automated CI/CD)
1. Push your changes to the repository
2. GitHub Actions automatically triggers the iOS build workflow
3. The workflow:
   - Builds the Unity project for iOS
   - Generates an Xcode project
   - Uploads build artifacts
4. Download artifacts from the Actions tab

**Build Configuration**: See `.github/workflows/unity-ios.yml`

### Build Parameters
```yaml
Build Target: iOS
Unity Version: 6000.0.57f1
Build Options: -nographics -silent-crashes
Architecture: ARM64
Minimum iOS: 14.0
```

---

## Testing

### Unity Play Mode Tests
```bash
# Run Unity Tests
# In Unity Editor: Window → General → Test Runner
# Click "Run All" in Play Mode tab
```

### Integration Testing
- **AR Session**: Test AR tracking in real-world environments
- **QR Codes**: Scan sample QR codes to verify detection
- **Search**: Test with various room names and partial queries
- **Navigation**: Walk through campus buildings to verify marker placement

---

### Quick Start
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Code Standards
- Follow Unity C# coding conventions
- Use meaningful variable and function names
- Add XML documentation comments to public methods
- Test your changes before submitting PR
- Keep commits atomic and well-described

---

## Team

**Rebel Reality Development Team - UNLV CS472/672**

| Name                  | Email                   | Role      |
|-----------------------|-------------------------|-----------|
| **Gerhod Moreno**     | moreng2@unlv.nevada.edu | Developer |
| **Bryan Duran**       | duranb@unlv.nevada.edu  | Developer |
| **Adrian Janda**      | jandaa1@unlv.nevada.edu | Developer |
| **Eshan Ahmad**       | ahmade1@unlv.nevada.edu | Developer |
| **Alex Yamasaki**     | yamasa1@unlv.nevada.edu | Developer |
| **Christopher Vuong** | vuongc2@unlv.nevada.edu | Developer |

---

## Acknowledgments
- **UNLV Computer Science Department** - Project sponsorship and support
- **Immersal** - Visual localization SDK
- **Unity Technologies** - Game engine and AR frameworks
- **Apple** - ARKit and Vision Framework

---

## Roadmap

### Current Features (v1.0)
- ✅ AR navigation with markers
- ✅ Room search and selection
- ✅ Dynamic label system
- ✅ QR code scanning
- ✅ OCR text recognition
- ✅ Multi-building support (AEB, TBE-A, TBE-B)

### Planned Features (v1.1+)
- 🔄 Pathfinding with Dijkstra's algorithm
- 🔄 Turn-by-turn navigation instructions
- 🔄 Indoor positioning without QR codes
- 🔄 Accessibility features (voice guidance)
- 🔄 Multi-floor navigation
- 🔄 Real-time location sharing
- 🔄 Offline map support

### Future Enhancements
- 📋 Android ARCore support
- 📋 Web-based map viewer
- 📋 Admin dashboard for map updates
- 📋 Analytics and usage statistics
- 📋 Integration with UNLV class schedules
- 📋 Outdoor Navigation 
---

## Known Issues

See [Issues](https://github.com/UNLV-CS472-672/2025-F-GROUP2-RR-XR/issues) for current bugs and feature requests.

### Common Issues
- **AR Session Not Starting**: Ensure location and camera permissions are granted
- **QR Codes Not Detected**: Ensure adequate lighting and steady camera
- **Labels Not Appearing**: Check AR mode is enabled and you're within proximity
- **Build Failures**: Verify Unity version matches exactly (6000.0.57f1)

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## Support

For questions, issues, or support:
- **GitHub Issues**: [Report a Bug](https://github.com/UNLV-CS472-672/2025-F-GROUP2-RR-XR/issues)
- **Email**: Contact team members listed above

---

**Built with ❤️ by the Rebel Reality Team at UNLV**

*Last Updated: November 2025*