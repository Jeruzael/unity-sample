using UnityEngine;

public class GPUTreeRenderer : MonoBehaviour {
    [Header("Connected Terrain")]
    public Terrain targetTerrain;         

    [Header("Assets Links")]
    public Mesh treeMesh;                 
    public Material treeMaterial;         
    public ComputeShader cullingShader;   

    private ComputeBuffer inputBuffer;
    private ComputeBuffer outputBuffer;
    private ComputeBuffer argsBuffer;
    
    private int kernelID;
    private int treeCount;

    void Start() {
        if (targetTerrain == null) {
            targetTerrain = GetComponentInParent<Terrain>();
            if (targetTerrain == null) targetTerrain = Terrain.activeTerrain;
        }

        if (targetTerrain == null || cullingShader == null) return;

        kernelID = cullingShader.FindKernel("CSMain");

        TerrainData terrainData = targetTerrain.terrainData;
        TreeInstance[] paintedTrees = terrainData.treeInstances;
        treeCount = paintedTrees.Length;

        if (treeCount == 0) return;

        int stride = sizeof(float) * 16; 
        inputBuffer = new ComputeBuffer(treeCount, stride);
        outputBuffer = new ComputeBuffer(treeCount, stride, ComputeBufferType.Append);
        
        argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
        uint[] args = new uint[5];
        args[0] = treeMesh != null ? treeMesh.GetIndexCount(0) : 0;
        args[1] = 0;
        args[2] = treeMesh != null ? treeMesh.GetIndexStart(0) : 0;
        args[3] = treeMesh != null ? treeMesh.GetBaseVertex(0) : 0;
        args[4] = 0;
        argsBuffer.SetData(args);

        Matrix4x4[] matrices = new Matrix4x4[treeCount];
        Vector3 terrainSize = terrainData.size;
        Vector3 terrainPos = targetTerrain.transform.position; 

        for (int i = 0; i < treeCount; i++) {
            TreeInstance instance = paintedTrees[i];

            Vector3 worldPos = new Vector3(
                instance.position.x * terrainSize.x,
                instance.position.y * terrainSize.y,
                instance.position.z * terrainSize.z
            ) + terrainPos;

            Quaternion rotation = Quaternion.Euler(0, instance.rotation * Mathf.Rad2Deg, 0);
            
            float scaleX = instance.widthScale > 0.01f ? instance.widthScale : 1f;
            float scaleY = instance.heightScale > 0.01f ? instance.heightScale : 1f;
            Vector3 scale = new Vector3(scaleX, scaleY, scaleX);

            matrices[i] = Matrix4x4.TRS(worldPos, rotation, scale);
        }
        
        inputBuffer.SetData(matrices);
        targetTerrain.drawTreesAndFoliage = false; 
    }

    void Update() {
        if (treeCount <= 0 || treeMesh == null || treeMaterial == null || cullingShader == null) return;

        // FIXED: Safe Camera Detection block to prevent the NullReferenceException
        Camera cam = Camera.main;
        if (cam == null) cam = FindFirstObjectByType<Camera>(); 
        if (cam == null) return; // Exit cleanly if absolutely no camera exists yet

        outputBuffer.SetCounterValue(0);
        
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        Vector4[] shaderPlanes = new Vector4[6];
        for (int i = 0; i < 6; i++) {
            shaderPlanes[i] = new Vector4(planes[i].normal.x, planes[i].normal.y, planes[i].normal.z, planes[i].distance);
        }
        
        cullingShader.SetBuffer(kernelID, "inputBuffers", inputBuffer);
        cullingShader.SetBuffer(kernelID, "outputBuffers", outputBuffer);
        cullingShader.SetVectorArray("cameraFrustumPlanes", shaderPlanes);
        
        int threadGroups = Mathf.CeilToInt(treeCount / 64f);
        cullingShader.Dispatch(kernelID, threadGroups, 1, 1);
        
        ComputeBuffer.CopyCount(outputBuffer, argsBuffer, sizeof(uint));
        treeMaterial.SetBuffer("visibleTreesBuffer", outputBuffer);
        
        Bounds renderBounds = new Bounds(Vector3.zero, Vector3.one * 100000f);
        Graphics.DrawMeshInstancedIndirect(treeMesh, 0, treeMaterial, renderBounds, argsBuffer);
    }

    void OnDestroy() {
        inputBuffer?.Release();
        outputBuffer?.Release();
        argsBuffer?.Release();
        
        if (targetTerrain != null) {
            targetTerrain.drawTreesAndFoliage = true;
        }
    }
}
