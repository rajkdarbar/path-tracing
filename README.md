# 🔆 Simple GPU Path Tracer

This project is a lightweight real-time **path tracer** built in Unity using compute shaders.  
The goal was to explore physically-based light transport — tracing stochastic rays, simulating multiple bounces, and progressively converging toward realistic illumination — all without using Unity’s built-in rendering pipeline.  

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

