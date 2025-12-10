
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
     1. Download a build from [ffmpega.org/download](https://ffmpeg.org/download.html).  
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



## 🚀 Usage
### 3D reconstruction:
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

Once you have the COLMAP output (`cameras.bin`, `images.bin`, `points3D.bin`, and `project.ini`), the next step is to run the **3D Gaussian Splatting reconstruction**.  

### 3D Gaussian Splatting
#### 📁 Required Files and Folders for 3DGS

Before running the Colab notebook, organize your data in the correct folder structure inside the `gaussian-splatting` project directory.

You need two folders:

1. **`input/`**  
   - Place the **original images** (frames extracted from your video) that were used for COLMAP. 

2. **`sparse/`**  
   - Place the **COLMAP output files** here. These files describe the reconstructed scene:
     - `cameras.bin`
     - `images.bin`
     - `points3D.bin`
     - `project.ini`  

#### Step 1 — Open Colab
- Launch the provided `3dgs.ipynb` notebook in Google Colab.
- Before running any code in the notebook:
1. Go to **Runtime → Change runtime type** in Colab.  
2. Set **Hardware accelerator** = **GPU (T4)**.  
3. Enable **High-RAM** option.  
   - This ensures there is enough memory for training large point clouds.  

- Run the cells **sequentially** until you reach the code block:

```python
# 파일을 저장할 경로를 지정
file_path = "/content/gaussian-splatting/requirements.txt"
content = "plyfile=0.8.1"

# 파일 작성
with open(file_path, "w") as f:
    f.write(content)

print(f"{file_path} 에 파일을 저장하였습니다")
```

This creates a file called requirements.txt inside /content/gaussian-splatting/.

#### Step 2 — Edit ```requirements.txt```
Open the newly created requirements.txt file in Colab.

Replace its contents with the following:
```
plyfile==0.8.1
tqdm
submodules/diff-gaussian-rasterization
submodules/simple-knn
```
#### Step 3 — Continue Running the Notebook

After saving the updated requirements.txt, continue running the remaining cells.

#### 📤 Output of 3DGS

After running the Colab notebook until the end, a new folder named **`output/`** will be created inside the project directory.  
This folder contains the trained Gaussian Splatting model.
project/3dgsOutput/point_cloud/iteration_50000/point_cloud.ply

- **`point_cloud.ply`** → This is the trained Gaussian Splatting model saved in PLY format.  

---

#### (Optional) Backend Integration
If you have a backend server running (for example, one that performs **object detection / KNN / image classification** using the 3DGS environment and communicates with Unity through the `CaptureAndSend` script on `127.0.0.1:8000`), you can use the following:

**Step 5 — Configure SendPhoto Object**
- In the scene hierarchy, select the **`SendPhoto`** object.  
- In the **Guide Contents** section, fill in:
- **Label** → the type of label expected from the backend  
- **Title** → title for the UI  
- **Description** → description text for the detected object  
- **Video (mp4)** → optional video clip  
- **TTS (wav)** → optional audio narration 
![Guide Content setting](unity_5.png)
---
