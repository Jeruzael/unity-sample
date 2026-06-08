using UnityEngine;

public class LowPolyTreeGenerator : MonoBehaviour
{
    [Header("Tree Settings")]
    public int treeCount = 50;
    public Vector2 areaSize = new Vector2(50f, 50f);

    [Header("Size Variation")]
    public float minHeight = 2.5f;
    public float maxHeight = 5f;

    [Header("Materials")]
    public Material trunkMaterial;
    public Material leavesMaterial;

    void Start()
    {
        GenerateTrees();
    }

    void GenerateTrees()
    {
        for (int i = 0; i < treeCount; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
                0f,
                Random.Range(-areaSize.y / 2f, areaSize.y / 2f)
            );

            CreateTree(position);
        }
    }

    void CreateTree(Vector3 position)
    {
        GameObject tree = new GameObject("Low Poly Tree");
        tree.transform.position = position;

        float height = Random.Range(minHeight, maxHeight);
        float trunkHeight = height * 0.45f;
        float leavesHeight = height * 0.65f;

        // Trunk
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.name = "Trunk";
        trunk.transform.parent = tree.transform;
        trunk.transform.localPosition = new Vector3(0f, trunkHeight / 2f, 0f);
        trunk.transform.localScale = new Vector3(0.25f, trunkHeight / 2f, 0.25f);

        if (trunkMaterial != null)
            trunk.GetComponent<Renderer>().material = trunkMaterial;

        // Make trunk low-poly looking
        Mesh trunkMesh = trunk.GetComponent<MeshFilter>().mesh;
        trunkMesh.RecalculateNormals();

        // Leaves
        GameObject leaves = CreateCone("Leaves", 8, 1.5f, leavesHeight);
        leaves.transform.parent = tree.transform;
        leaves.transform.localPosition = new Vector3(0f, trunkHeight + leavesHeight / 2f - 0.2f, 0f);
        leaves.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        float leafScale = Random.Range(0.8f, 1.3f);
        leaves.transform.localScale = new Vector3(leafScale, 1f, leafScale);

        if (leavesMaterial != null)
            leaves.GetComponent<Renderer>().material = leavesMaterial;

        // Random rotation
        tree.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
    }

    GameObject CreateCone(string name, int segments, float radius, float height)
    {
        GameObject cone = new GameObject(name);

        MeshFilter meshFilter = cone.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = cone.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 6];

        // Top point
        vertices[0] = new Vector3(0f, height / 2f, 0f);

        // Center bottom point
        vertices[1] = new Vector3(0f, -height / 2f, 0f);

        // Bottom circle points
        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            vertices[i + 2] = new Vector3(x, -height / 2f, z);
        }

        int triangleIndex = 0;

        for (int i = 0; i < segments; i++)
        {
            int current = i + 2;
            int next = ((i + 1) % segments) + 2;

            // Side triangle
            triangles[triangleIndex++] = 0;
            triangles[triangleIndex++] = next;
            triangles[triangleIndex++] = current;

            // Bottom triangle
            triangles[triangleIndex++] = 1;
            triangles[triangleIndex++] = current;
            triangles[triangleIndex++] = next;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        return cone;
    }
}