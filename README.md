# 🔆 Simple GPU Path Tracer

The goal of this mini project is to understand the fundamentals of **path tracing** using Unity’s compute shaders. It focuses on the core ideas of **physically-based light transport** — tracing rays, simulating surface interactions, and progressively converging toward realistic illumination — all without relying on Unity’s built-in rendering pipeline.

---

## 🧩 What This Project Demonstrates

- **Fully GPU-accelerated path tracer** implemented with Unity compute shaders  
- Renders **procedural spheres**, a **ground plane**, and **low-poly mesh geometry**  
- Uses **unbiased Monte Carlo integration** with **importance sampling** for efficient light transport  
- Implements **Material-specific sampling:**  
&nbsp;&nbsp;&nbsp;• Diffuse → cosine-weighted hemisphere sampling  
&nbsp;&nbsp;&nbsp;• Glossy metallic → Phong-lobe specular sampling  
&nbsp;&nbsp;&nbsp;• Glossy dielectric → probabilistic mix of diffuse / specular (Russian Roulette)  
&nbsp;&nbsp;&nbsp;• Emissive surfaces → direct light sources  
- **Russian Roulette path termination** for unbiased, efficient multi-bounce tracing  
- **Per-pixel randomness** (pixel jitter + random seed) for decorrelated sampling across frames  
- **Progressive sample accumulation** via a custom blending shader for noise reduction  
- Produces realistic effects such as **indirect lighting**, **soft reflections**, **soft shadows**, and **environment-based illumination**  
- Supports multiple **camera models** — Perspective, Orthographic, Fisheye, and Panoramic  

---

## 🖼️ Path Traced Sample Output Images

### 💠 Material-Based Scenes

<div align="left">
  <img src="Assets/Resources/Output Images/pathtracing-diffuse-scene.png" width="290">
  <img src="Assets/Resources/Output Images/pathtracing-glossy-dielectric-scene.png" width="290">
</div>

<div align="left">
  <img src="Assets/Resources/Output Images/pathtracing-glossy-metallic-scene.png" width="290">
  <img src="Assets/Resources/Output Images/pathtracing-mixed-scene.png" width="290">
</div>

**Top-Left:** Diffuse Scene  
**Top-Right:** Glossy Dielectric Scene  
**Bottom-Left:** Glossy Metallic Scene  
**Bottom-Right:** Mixed Scene — Diffuse, Dielectric, & Metallic  

### 📷 Scenes Rendered with Different Camera Projections & Lens Effects

<div align="left">
  <img src="Assets/Resources/Output Images/pathtracing-orthographic-view.png" width="290">
  <img src="Assets/Resources/Output Images/pathtracing-fisheye-effect.png" width="293">
</div>

<div align="left">
  <img src="Assets/Resources/Output Images/pathtracing-panoramic-effect.png" width="588">
</div>

**Top-Left:** Orthographic Camera Projection  
**Top-Right:** Fisheye Lens Effect  
**Bottom:** Panoramic Lens Effect

---

## 🎥 Demo Video

Don't forget to take a look at [this video](https://youtu.be/wOaUeJfeyXY) running on an **NVIDIA GeForce RTX 3070 Laptop GPU**.

---

## ⚠️ Limitations & Future Improvements

- The current implementation works best with **low-poly meshes** (a few hundred triangles). High-poly models slow down rendering due to per-triangle intersection checks and could be significantly improved with **Bounding Volume Hierarchy (BVH)** acceleration.

- This implementation is **not real-time** — each frame takes several minutes to converge. **Denoising** could be used for possible enhancement.

- It lacks **Multiple Importance Sampling (MIS)** for more accurate global illumination.

---

## 🙏 Credits

Special thanks to [**David Kuri**](https://twitter.com/davidjkuri) for his outstanding GPU Path Tracing tutorial series, which served as the main inspiration for this project. 

- 📙 [Tutorial – GPU Path Tracing in Unity (Part 2)](https://web.archive.org/web/20230926231248/http://three-eyed-games.com/2018/05/12/gpu-path-tracing-in-unity-part-2/)  
- 🖥️ [Code – Unity Path Tracing Tutorial (Part 2)](https://bitbucket.org/Daerst/gpu-ray-tracing-in-unity/src/Tutorial_Pt2/)  
- 📙 [Tutorial – GPU Path Tracing in Unity (Part 3)](https://web.archive.org/web/20230926225937/http://three-eyed-games.com/2019/03/18/gpu-path-tracing-in-unity-part-3/)  
- 🖥️ [Code – Unity Path Tracing Tutorial (Part 3)](https://bitbucket.org/Daerst/gpu-ray-tracing-in-unity/src/Tutorial_Pt3/)














