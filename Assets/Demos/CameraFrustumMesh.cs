using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFrustumMesh : MonoBehaviour
{
    private MeshFilter meshFilter;
    private Mesh frustumMesh;
    private Camera cam;
    public Material frustumMaterial;

    void Start()
    {
        cam = GetComponent<Camera>();
        GameObject meshObject = new GameObject("FrustumMesh");
        meshObject.transform.SetParent(transform, false);

        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        var meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshRenderer.material = frustumMaterial != null ? frustumMaterial : new Material(Shader.Find("Standard"));
        meshRenderer.gameObject.layer = gameObject.layer;
        frustumMesh = meshFilter.mesh;
    }

    void Update()
    {
        UpdateFrustumMesh();
    }

    void UpdateFrustumMesh()
    {
        frustumMesh.Clear();

        Vector3[] frustumCorners = new Vector3[8];
        Vector2[] uvs = new Vector2[8];
        Matrix4x4 camLocalToWorld = cam.transform.localToWorldMatrix;

        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.farClipPlane - 0.1f, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);
        for (int i = 0; i < 4; i++)
        {
            frustumCorners[i + 4] =  frustumCorners[i];
            uvs[i + 4] = new Vector2(i % 2, 0); // Far plane UV
        }

        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);
        for (int i = 0; i < 4; i++)
        {
            frustumCorners[i] = frustumCorners[i];
            uvs[i] = new Vector2(i % 2, 1); // Near plane UV
        }

        frustumMesh.vertices = frustumCorners;
        frustumMesh.uv = uvs;

        int[] triangles = new int[]
        {
            0, 1, 2,  2, 3, 0, // Near plane
            4, 7, 6,  6, 5, 4, // Far plane
            0, 4, 5,  5, 1, 0, // Left plane
            1, 5, 6,  6, 2, 1, // Bottom plane
            2, 6, 7,  7, 3, 2, // Right plane
            3, 7, 4,  4, 0, 3  // Top plane
        };

        frustumMesh.triangles = triangles;
        frustumMesh.RecalculateNormals();
    }
}