# 🔆 Simple GPU Path Tracer

The goal of this mini project is to understand the fundamentals of **path tracing** using Unity’s compute shaders.  
It focuses on the core ideas of **physically-based light transport** — tracing rays, simulating surface interactions, and progressively converging toward realistic illumination — all without relying on Unity’s built-in rendering pipeline.

---

## 🧩 What This Project Demonstrates

✅ **Fully GPU-accelerated path tracer** implemented with Unity compute shaders  
✅ Renders **procedural spheres**, a **ground plane**, and **low-poly mesh geometry**  
✅ Uses **unbiased Monte Carlo integration** with **importance sampling** for efficient light transport  
✅ Implements **Material-specific sampling:**  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;• Diffuse → cosine-weighted hemisphere sampling  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;• Glossy metallic → Phong-lobe specular sampling  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;• Glossy dielectric → probabilistic mix of diffuse / specular (Russian Roulette)  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;• Emissive surfaces → direct light sources  
✅ **Russian Roulette path termination** for unbiased, efficient multi-bounce tracing  
✅ **Per-pixel randomness** (pixel jitter + random seed) for decorrelated sampling across frames  
✅ **Progressive sample accumulation** via a custom blending shader for noise reduction  
✅ Produces realistic effects such as **indirect lighting**, **soft reflections**, **soft shadows**, and **environment-based illumination**  
✅ Supports multiple **camera models** — Perspective, Orthographic, Fisheye, and Panoramic  

---

## 🖼️ Sample Path Traced Output Images

### 🟦 Diffuse Scene
<img src="Assets/Resources/Output Images/pathtracing-diffuse-scene.png" width="600">

<br>

### 🟪 Glossy Dielectric Scene
<img src="Assets/Resources/Output Images/pathtracing-glossy-dielectric-scene.png" width="600">

<br>

### 🟨 Glossy Metallic Scene
<img src="Assets/Resources/Output Images/pathtracing-glossy-metallic-scene.png" width="600">

<br>

### 🟧 Mixed Scene — Diffuse, Dielectric, & Metallic
<img src="Assets/Resources/Output Images/pathtracing-mixed-scene.png" width="600">

<br>

### 🟩 Rendered Scene with Orthographic Camera Projection
<img src="Assets/Resources/Output Images/pathtracing-orthographic-view.png" width="600">

<br>

### 🟥 Rendered Scene with Fisheye Lens Effect
<img src="Assets/Resources/Output Images/pathtracing-fisheye-effect.png" width="600">

<br>

### 🟫 Rendered Scene with Panoramic Lens Effect
<img src="Assets/Resources/Output Images/pathtracing-panoramic-effect.png" width="600">

<br>


