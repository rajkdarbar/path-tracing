using System.Collections.Generic;
using UnityEngine;

public class PathTracingMaster : MonoBehaviour
{
    public ComputeShader PathTracingShader;
    public Texture SkyboxTexture;
    public Light DirectionalLight;

    public bool RegenerateScene = false;

    [Header("Spheres")]
    public Vector2 SphereRadius = new Vector2(3.0f, 8.0f);
    public uint SpheresMax = 100;
    public float SpherePlacementRadius = 100.0f;

    [Header("3D Meshes")]
    public GameObject[] MeshObjects;

    private Camera mainCamera;
    private float lastFieldOfView;

    private RenderTexture targetRT;
    private RenderTexture convergedRT;

    private uint currentSample = 0;
    private Material addMaterial;

    private ComputeBuffer sphereBuffer;
    private ComputeBuffer triangleBuffer;

    private List<Transform> trackedTransforms = new List<Transform>();

    struct Sphere
    {
        public Vector3 position; // 12 bytes
        public float radius; // 4 bytes
        public Vector3 albedo; // 12 bytes
        public Vector3 specular; // 12 bytes
        public float smoothness; // 4 bytes
        public Vector3 emission; // 12 bytes
    }

    struct Triangle
    {
        public Vector3 v0; // 12 bytes
        public Vector3 v1; // 12 bytes
        public Vector3 v2; // 12 bytes
        public Vector3 normal; // 12 bytes
        public Vector3 albedo; // 12 bytes
        public Vector3 specular; // 12 bytes
        public float smoothness; // 4 bytes
        public Vector3 emission; // 12 bytes
    }


    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        currentSample = 0;
        SetUpScene();
    }

    private void OnDisable()
    {
        if (sphereBuffer != null)
            sphereBuffer.Release();

        if (triangleBuffer != null)
            triangleBuffer.Release();

    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        SetShaderParameters();
        Render(destination);
    }

    void Update()
    {
        //VisualizeRays();

        if (RegenerateScene)
        {
            RegenerateScene = false;
            SetUpScene();
            currentSample = 0;
        }

        if (mainCamera.fieldOfView != lastFieldOfView)
        {
            currentSample = 0;
            lastFieldOfView = mainCamera.fieldOfView;
        }

        bool needsRebuild = false;

        foreach (Transform t in trackedTransforms)
        {
            if (t.hasChanged)
            {
                needsRebuild = true;
                t.hasChanged = false;
            }
        }

        if (needsRebuild)
        {
            SetUpScene();
            currentSample = 0;
        }
    }

    private void VisualizeRays()
    {
        int width = Screen.width;
        int height = Screen.height;

        for (int x = 0; x < width; x += 100)
        {
            for (int y = 0; y < height; y += 100)
            {
                Vector2 uv = new Vector2((float)x / width, (float)y / height);
                Ray ray = mainCamera.ViewportPointToRay(uv);
                Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
            }
        }
    }

    private void SetUpScene()
    {
        trackedTransforms.Clear();

        // Always track camera + light
        trackedTransforms.Add(transform);
        trackedTransforms.Add(DirectionalLight.transform);

        // Part - I: Setup Spheres
        List<Sphere> spheres = new List<Sphere>();
        const int MAX_RETRIES = 10;

        for (int i = 0; i < SpheresMax; i++)
        {
            int retries = 0;
            Sphere sphere;

        RetryPlacement:
            sphere = new Sphere();
            sphere.radius = SphereRadius.x + Random.value * (SphereRadius.y - SphereRadius.x);

            Vector2 randomPos = Random.insideUnitCircle * SpherePlacementRadius;
            sphere.position = new Vector3(randomPos.x, sphere.radius, randomPos.y);

            // Check overlap
            foreach (Sphere other in spheres)
            {
                float minDist = sphere.radius + other.radius;
                if (Vector3.SqrMagnitude(sphere.position - other.position) < minDist * minDist)
                {
                    if (retries++ < MAX_RETRIES)
                        goto RetryPlacement; // try again
                    else
                        goto SkipSphere; // give up on this one
                }
            }

            Color color = Random.ColorHSV();

            float materialSelector = Random.value;
            if (materialSelector < 0.40f)
            {
                // DIFFUSE (Lambertian) — rough, matte. 
                sphere.albedo = new Vector3(color.r, color.g, color.b);
                sphere.specular = new Vector3(0.04f, 0.04f, 0.04f);
                sphere.smoothness = 0.0f;
                sphere.emission = Vector3.zero;
            }

            else if (materialSelector >= 0.4f && materialSelector < 0.6f)
            {
                // GLOSSY DIELECTRIC — plastic, ceramic, smooth surface.
                sphere.albedo = new Vector3(color.r, color.g, color.b);
                sphere.specular = new Vector3(1f, 1f, 1f); // white highlights
                sphere.smoothness = Random.Range(0.2f, 0.6f);
                sphere.emission = Vector3.zero;
            }

            else if (materialSelector >= 0.6f && materialSelector < 0.9f)
            {
                // GLOSSY METALLIC — shiny metal, color-tinted reflections.
                sphere.albedo = Vector3.zero;
                sphere.specular = new Vector3(color.r, color.g, color.b);
                sphere.smoothness = Random.Range(0.6f, 1.0f);
                sphere.emission = Vector3.zero;
            }

            else
            {
                // EMISSIVE — light source.
                Color emissionColor = UnityEngine.Random.ColorHSV(0f, 1f, 0f, 1f, 1f, 3.5f);

                sphere.albedo = Vector3.zero;
                sphere.specular = Vector3.zero;
                sphere.smoothness = 0f;
                sphere.emission = new Vector3(emissionColor.r, emissionColor.g, emissionColor.b);
            }

            // Add the sphere to the list
            spheres.Add(sphere);
            continue;

        SkipSphere:
            continue;
        }

        // Assign spheres to compute buffer
        if (sphereBuffer != null)
            sphereBuffer.Release();

        if (spheres.Count > 0)
        {
            const int SphereStride = sizeof(float) * (3 + 1 + 3 + 3 + 1 + 3); // 56 bytes
            sphereBuffer = new ComputeBuffer(spheres.Count, SphereStride);
            sphereBuffer.SetData(spheres);
        }


        // Part II - Setup 3D Mesh Geometry        
        List<Triangle> triangles = new List<Triangle>();

        foreach (var obj in MeshObjects)
        {
            if (obj == null) continue;
            trackedTransforms.Add(obj.transform);

            var meshFilter = obj.GetComponentInChildren<MeshFilter>();
            var renderer = obj.GetComponentInChildren<MeshRenderer>();
            if (meshFilter == null || renderer == null) continue;

            Mesh mesh = meshFilter.sharedMesh;
            if (mesh == null) continue;

            var verts = mesh.vertices;
            var indices = mesh.triangles;
            var localToWorld = meshFilter.transform.localToWorldMatrix;

            // Extract material properties
            var mat = renderer.sharedMaterial;
            Color albedoColor = mat.HasProperty("_Color") ? mat.color : Color.white;
            float metallic = mat.HasProperty("_Metallic") ? mat.GetFloat("_Metallic") : 0f;
            float smoothness = mat.HasProperty("_Glossiness") ? mat.GetFloat("_Glossiness") : 0.5f;
            Color emissionColor = mat.IsKeywordEnabled("_EMISSION") ? mat.GetColor("_EmissionColor") : Color.black;

            Vector3 albedo = Vector3.zero;
            Vector3 specular = Vector3.zero;
            if (metallic > 0.5f)
            {
                // Glossy metallic
                specular = new Vector3(albedoColor.r, albedoColor.g, albedoColor.b);
            }
            else
            {
                // Diffuse or glossy dielectric
                albedo = new Vector3(albedoColor.r, albedoColor.g, albedoColor.b);
                specular = (smoothness > 0.0f) ? new Vector3(1, 1, 1) : new Vector3(0.04f, 0.04f, 0.04f);
            }

            Vector3 emission = new Vector3(emissionColor.r, emissionColor.g, emissionColor.b);

            for (int i = 0; i < indices.Length; i += 3)
            {
                Triangle tri = new Triangle();

                tri.v0 = localToWorld.MultiplyPoint(verts[indices[i]]);
                tri.v1 = localToWorld.MultiplyPoint(verts[indices[i + 1]]);
                tri.v2 = localToWorld.MultiplyPoint(verts[indices[i + 2]]);
                tri.normal = Vector3.Cross(tri.v1 - tri.v0, tri.v2 - tri.v0).normalized;

                tri.albedo = albedo;
                tri.specular = specular;
                tri.smoothness = smoothness;
                tri.emission = emission;

                triangles.Add(tri);
            }
        }

        // Assign triangles to compute buffer
        if (triangleBuffer != null)
            triangleBuffer.Release();

        if (triangles.Count > 0)
        {
            const int stride = sizeof(float) * (3 + 3 + 3 + 3 + 3 + 3 + 1 + 3); // 88 bytes
            triangleBuffer = new ComputeBuffer(triangles.Count, stride);
            triangleBuffer.SetData(triangles);
        }
    }

    private void SetShaderParameters()
    {
        PathTracingShader.SetFloat("seed", Random.value);
        PathTracingShader.SetTexture(0, "SkyboxTexture", SkyboxTexture);
        PathTracingShader.SetMatrix("CameraToWorld", mainCamera.cameraToWorldMatrix);
        PathTracingShader.SetMatrix("CameraInverseProjection", mainCamera.projectionMatrix.inverse);
        PathTracingShader.SetVector("PixelOffset", new Vector2(Random.value, Random.value));

        Vector3 light = DirectionalLight.transform.forward;
        PathTracingShader.SetVector("DirectionalLight", new Vector4(light.x, light.y, light.z, DirectionalLight.intensity));

        if (sphereBuffer != null && sphereBuffer.IsValid())
            PathTracingShader.SetBuffer(0, "SpheresBuffer", sphereBuffer);

        if (triangleBuffer != null && triangleBuffer.IsValid())
            PathTracingShader.SetBuffer(0, "TrianglesBuffer", triangleBuffer);


    }

    private void Render(RenderTexture destination)
    {
        InitRenderTexture();

        PathTracingShader.SetTexture(0, "Result", targetRT);

        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);
        PathTracingShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        if (addMaterial == null)
            addMaterial = new Material(Shader.Find("Hidden/AddShader"));

        RenderTexture tempRT = RenderTexture.GetTemporary(convergedRT.width, convergedRT.height, 0, RenderTextureFormat.ARGBFloat);

        addMaterial.SetTexture("_MainTex", targetRT); // new sample
        addMaterial.SetTexture("_History", convergedRT); // previous frame
        addMaterial.SetFloat("_Sample", currentSample);

        Graphics.Blit(null, tempRT, addMaterial); // shader output gets stored in tempRT        
        Graphics.Blit(tempRT, convergedRT); // copy back from tempRT to convergedRT
        Graphics.Blit(convergedRT, destination);
        RenderTexture.ReleaseTemporary(tempRT);

        currentSample++;
    }


    private void InitRenderTexture()
    {
        if (targetRT == null || targetRT.width != Screen.width || targetRT.height != Screen.height)
        {
            if (targetRT != null)
            {
                targetRT.Release();
            }

            if (convergedRT != null)
            {
                convergedRT.Release();
            }

            targetRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            targetRT.enableRandomWrite = true;
            targetRT.Create();

            convergedRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            convergedRT.enableRandomWrite = true;
            convergedRT.Create();

            currentSample = 0;
        }
    }
}