# Photorealistic VR Tour using 3D Gaussian Splatting
### 📌 Introduction

This project aims to create photorealistic VR tours by combining 3D Reconstruction (COLMAP, OpenMVS) with 3D Gaussian Splatting (3DGS), and deploying the results in Unity for VR devices (Meta Quest 3, PCVR).  
Unlike existing VR tours limited to pre-modeled spaces, our approach allows users to capture any environment with a smartphone camera and transform it into an immersive, high-quality VR tour.

### 🎯 Motivation  
Traditional VR tours are either:  
- Panoramic-based → limited to static viewing points.  
- Manually modeled in 3D engines → high cost & time-consuming.  

**Our goal**: enable fast, automated, and high-quality VR tour creation from real-world imagery while maintaining strong photorealism.

### 📋 index
- [Photorealistic VR Tour using 3D Gaussian Splatting](#photorealistic-vr-tour-using-3d-gaussian-splatting)
- [Pipeline](#pipeline)
- [Technical Requirements](#-technical-requirements)
  - [Hardware](#hardware)
  - [Software](#software)
- [Installation & Setup](#-installation--setup)
  - [Data](#1-data)
  - [Colmap](#2-colmap)
  - [3dgs](#3-3dgs)
  - [Unity](#4-unity)
- [Usage](#-usage)
  - [Quick Start](#quick-start)
  - [Output](#output)
  - [VR](#vr)
- [Future Work](#-future-work)
- [Contribution](#contribution)
- [Citation](#citation)
- [License info](#license-info)

## Pipeline

1. Image Capture → User records a short (1–3 min) video of the target environment.

2. 3D Reconstruction (COLMAP) → Sparse point clouds + camera poses.

4. 3D Gaussian Splatting (3DGS) → Optimized point cloud rendering with high realism.

5. Unity Integration → Export to VR scene, interactive with Meta Quest 3.

## 💻 Technical Requirements
### Hardware
GPU: NVIDIA RTX with ≥ 16 GB VRAM (recommended 24 GB+ for large scenes).  
CPU: 8+ cores.  
RAM: 32 GB+.  
Storage: ~100 GB free space.  
VR Device: Meta Quest 3 (PCVR recommended.  

### Software
OS: Windows 10.  
Python 3.10+ (Conda environment provided).  
CUDA 11.8+.  
Unity 2022+ with XR Interaction Toolkit.  

### Prerequisites

## 🔧 Installation & Setup


### 1. Data 
The pipeline requires image sequences as input. You can either use the provided **sample dataset** or prepare your own data from a recorded video.

### 🔹 Sample Data
To quickly test the pipeline:
- Download the provided sample dataset: *link* .
- Place it inside the `datasets/` folder.  

 
### 🔹 Custom Data
To process your own environment:

1. **Record a Video**
 - Use a smartphone or camera.  
 - Recommended: **1–3 minutes** of video while slowly walking around the target environment.  
 - Capture smooth motion with overlapping views (avoid rapid rotations).  

2. **Extract Frames**
 - Install [FFmpeg](https://ffmpeg.org/):
   - **macOS (Homebrew):**
     ```bash
     brew install ffmpeg
     ```
   - **Windows:**
     1. Download a build from [ffmpeg.org/download](https://ffmpeg.org/download.html).  
     2. Extract it and add the `bin` folder to your system PATH.  

 - Run the command to extract frames at 2–5 fps:
   	- macOS:
   ```bash
   ffmpeg -i video.mp4 -vf fps=3 frames/v_%04d.jpg
   ```
   -  Windows CMD: use backslashes instead of forward slashes:
   ```cmd
   ffmpeg -i video.mp4 -vf fps=3 frames\v_%04d.jpg
   ```
   - Adjust fps depending on:
	   - **Simple scenes** → 2 fps is enough.  
	   - **Complex/detailed scenes** → use 4–5 fps for better reconstruction.
3. **Organize Dataset**
 - Place your extracted frames into the project folder:
   ```
   datasets/<scene_name>/frames/
   ```
	
### 2. Colmap
👉 Download and install COLMAP. 
Follow the [installation guidelines](https://colmap.github.io/install.html) .

### 3. 3dgs


### 4. Unity 



## 🚀 Usage
### Quick Start:
Follow the COLMAP [tutorial](https://colmap.github.io/tutorial.html) to get started with reconstruction.
#### Step 1 — Open COLMAP
- Launch the [COLMAP GUI](https://colmap.github.io/gui.html#gui). 
#### Step 2 — Create a New Project
- Go to **File → New Project…**  
- Set:
  - **Image path:** `datasets/<scene_name>/frames/`
  - **Database path:** `datasets/<scene_name>/colmap/database.db`
  - **Project file:** (optional, e.g. `datasets/<scene_name>/colmap/project.ini`)  
- Click **Save**.
#### Step 3 — Feature Extraction
- From the top menu: **Processing → Feature Extraction…**
- Select:
  - **Camera model:** `SIMPLE_RADIAL` (recommended for smartphone video)  
  - **Single camera:** enabled  
  - **Use GPU:** if available  
- Run the extraction.
#### Step 4 — Feature Matching
- For video frames (sequential order): **Processing → Sequential Matcher…**
  - Set *Overlap* = 5–10 (controls how many neighboring frames to match).  
- For unordered photos: use **Exhaustive Matcher** instead.  
- Run the matching.
#### Step 5 — Sparse Reconstruction (Mapping)
- From the top menu: **Reconstruction → Start Reconstruction…**
- Select:
  - **Database path** = your project’s `database.db`  
  - **Image path** = `frames/`  
  - **Output path** = `datasets/<scene_name>/colmap/sparse/`  
- Click **Run**. COLMAP will create one or more models (e.g. `sparse/0`).
#### Step 6 — Save the Model for Next Step
COLMAP automatically saves the reconstruction in **binary format**, which is required for 3D Gaussian Splatting.
After reconstruction, you should export model and get the following files:
- `cameras.bin`
- `images.bin`
- `points3D.bin`

Additionally, keep your `project.ini` (created when you made the project).  
👉 These files will be used directly as input for the **3D Gaussian Splatting** stage.  

### Output: 
### VR:

## 🔮 Future Work

## Contribution
## Citation
## License info

👥 Team  **"RealityOne"**  
조석현 (202011371)  
최지야 (202213586)  
하라다카호 (202213528)  


