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
- [Photorealistic VR Tour using 3D Gaussian Splatting](#Photorealistic-VR-Tour-using-3D-Gaussian-Splatting)
- [What is 3D Reconstruction?](#What-is-3D-Reconstruction?)
- [What is Gaussian Splatting?](#What-is-Gaussian-Splatting?)
- [Pipeline](#Pipeline)
- [Technical Requirements](#technical-requirements)
	- [Hardware](#Hardware)
 	- [Software](#Software)
- [Installation & Setup](#Installation-&-Setup)
	- [Data](#Data)
 	- [Colmap](#Colmap)
    - [3dgs](#3dgs)
    - [Unity](#Unity)
- [Usage](#Usage)
	- [Quick Start](#Quick-Start)
 	- [Output](#Output)
  	- [VR](#VR)
- [Future Work](#Future-Work)
- [Contribution](#Contribution)
- [Citation](#Citation)
- [License info](#License-info)


## ❓ What is 3D Reconstruction?
3D Reconstruction is the process of creating a **3D digital model** of a real-world object or environment from 2D images or videos.  
Tool like **COLMAP** analyze multiple photos taken from different angles.  
It extracts **feature points** (edges, corners, textures), estimate **camera positions**, and build a **point cloud**.  

From this point cloud, a mesh with textures can be generated, forming a digital replica of the scene.  
👉 In our project, 3D reconstruction provides the **geometric structure** of the environment.

## ❓ What is Gaussian Splatting? 
3D Gaussian Splatting (3DGS) is a **recent rendering technique** for photorealistic 3D scenes.  
Instead of meshes, it represents the world as **millions of Gaussian “blobs”** in 3D space.  
Each Gaussian carries **position, color, size, orientation, and transparency**.  
During rendering, these blobs are projected (“splatted”) onto the screen and blended together.  

The result: **smooth, high-quality visuals** with realistic lighting and textures — often better and faster than traditional NeRFs or meshes.  
👉 In our project, 3DGS enhances the reconstructed geometry to deliver **photorealism inside VR**.

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
Sample Data:  
Download the provided sample dataset (/samples/hwangso/) to test the pipeline.  
Custom Data:  
- Record a video (1–3 min).  
- Extract frames at 2–5 fps.
Place into datasets/<scene_name>/images/.
	
### 2. Colmap
Follow the instructions for Colmap installation on the official page:  
https://colmap.github.io/install.html
### 3. 3dgs

### 4. Unity 



## 🚀 Usage
### Quick Start:
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


