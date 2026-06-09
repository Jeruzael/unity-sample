using UnityEngine;
using UnityEditor;

public static class MeshToTerrainContext
{
    // This adds a right-click option directly onto GameObjects in the Hierarchy
    [MenuItem("GameObject/Convert This Mesh to Terrain", false, 10)]
    public static void ConvertSelectedMesh(MenuCommand menuCommand)
    {
        GameObject sourceMesh = menuCommand.context as GameObject;
        if (sourceMesh == null) return;

        // Ensure the object actually has a mesh to read
        MeshFilter filter = sourceMesh.GetComponentInChildren<MeshFilter>();
        if (filter == null)
        {
            EditorUtility.DisplayDialog("Error", $"'{sourceMesh.name}' or its children do not have a Mesh Filter component!", "OK");
            return;
        }

        // Setup collider for raycasting
        MeshCollider meshCollider = sourceMesh.GetComponent<MeshCollider>();
        bool addedCollider = false;
        if (meshCollider == null)
        {
            meshCollider = sourceMesh.AddComponent<MeshCollider>();
            addedCollider = true;
        }

        Bounds bounds = meshCollider.bounds;
        int res = 513; // Default balanced resolution (512 + 1)

        TerrainData terrainData = new TerrainData();
        terrainData.heightmapResolution = res;
        terrainData.size = new Vector3(bounds.size.x, bounds.size.y * 1.2f, bounds.size.z);

        float[,] heights = new float[res, res];
        float stepX = bounds.size.x / (res - 1);
        float stepZ = bounds.size.z / (res - 1);

        EditorUtility.DisplayProgressBar("Converting Mesh", "Scanning mesh geometry...", 0f);

        for (int z = 0; z < res; z++)
        {
            for (int x = 0; x < res; x++)
            {
                float rayX = bounds.min.x + (x * stepX);
                float rayZ = bounds.min.z + (z * stepZ);
                float rayY = bounds.max.y + 10f;

                Ray ray = new Ray(new Vector3(rayX, rayY, rayZ), Vector3.down);
                RaycastHit hit;

                if (meshCollider.Raycast(ray, out hit, bounds.size.y + 20f))
                {
                    float localHitY = hit.point.y - bounds.min.y;
                    heights[z, x] = Mathf.Clamp01(localHitY / terrainData.size.y);
                }
                else
                {
                    heights[z, x] = 0f;
                }
            }
        }

        terrainData.SetHeights(0, 0, heights);
        
        // Save asset dynamically using the mesh name
        string assetPath = $"Assets/{sourceMesh.name}_TerrainData.asset";
        AssetDatabase.CreateAsset(terrainData, assetPath);
        AssetDatabase.SaveAssets();

        // Spawn terrain exactly where the object is
        GameObject terrainGO = Terrain.CreateTerrainGameObject(terrainData);
        terrainGO.name = sourceMesh.name + "_Terrain";
        terrainGO.transform.position = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z);

        // Deactivate original mesh automatically so you instantly see the terrain
        Undo.RegisterCompleteObjectUndo(sourceMesh, "Disable original mesh");
        sourceMesh.SetActive(false);

        if (addedCollider) Object.DestroyImmediate(meshCollider);
        EditorUtility.ClearProgressBar();
    }
}
