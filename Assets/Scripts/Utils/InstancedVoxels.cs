using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InstancedVoxels : MonoBehaviour
{
    public GameObject prefab;
    public Material meshMaterial;
    public float gridSize;
    public float timeToLive;
    public float spacing;
    private MeshFilter mMeshFilter;
    private MeshRenderer mMeshRenderer;
    private BoxCollider mCollider;
    private Dictionary<Vector3, Vector2> voxelGrid;
    private Matrix4x4[] matrices;
    public void Round(ref Vector3 v)
    {
        v.x = Mathf.Round(v.x);
        v.y = Mathf.Round(v.y);
        v.z = Mathf.Round(v.z);
    }
    void Start ()
    {
        mMeshFilter = prefab.GetComponent<MeshFilter>();
        mMeshRenderer = prefab.GetComponent<MeshRenderer>();
        mCollider = GetComponent<BoxCollider>();

        voxelGrid = new Dictionary<Vector3, Vector2>();
    }

    private Matrix4x4[] GetPointsInsideOfCollider()
    {
        int width = Mathf.FloorToInt(2f * mCollider.bounds.extents.x/gridSize);
        int height = Mathf.FloorToInt(2f * mCollider.bounds.extents.y/gridSize);
        int depth = Mathf.FloorToInt(2f * mCollider.bounds.extents.z/gridSize);

        float sqRadius = mCollider.bounds.extents.x * mCollider.bounds.extents.x;
        
        Vector3 scale = spacing * gridSize * new Vector3(1, 1, 1);
        Vector3 starting = mCollider.bounds.center - mCollider.bounds.extents;
        // round the starting position to our quantized grid 
        starting /= gridSize;
        Round(ref starting);
        starting *= gridSize;
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                for (int k = 0; k < depth; k ++) 
                {
                    Vector3 pos = starting + new Vector3(i * gridSize, j * gridSize, k*gridSize);
                    if(Vector3.SqrMagnitude(mCollider.bounds.center - pos) > sqRadius) continue;
                    if(!voxelGrid.ContainsKey(pos))
                    {
                        voxelGrid.Add(pos, new Vector2());
                    }
                    Vector2 val = voxelGrid[pos];
                    val.x = Mathf.Min(val.x + Time.deltaTime, timeToLive);
                    val.y = 1f;
                    voxelGrid[pos] = val;
                }
            }
        }

        int idx = 0;
        matrices = new Matrix4x4[voxelGrid.Count];
        foreach(Vector3 pos in voxelGrid.Keys.ToList())
        {
            Vector2 val = voxelGrid[pos];
            if(val.y < 1f)
            {
                val.x -= Time.deltaTime;
            }
            val.y = 0f;
            if(val.x <= 0f)  
            {
               voxelGrid.Remove(pos);
            }
            else 
            {
                voxelGrid[pos] = val;
            }
            matrices[idx] = Matrix4x4.identity;
            matrices[idx].SetTRS(pos, Quaternion.identity, (val.x/timeToLive) * scale);
            idx ++;
        }
        return matrices;
    }

    void Update ()
    {
        Matrix4x4[] batchedMatrices = GetPointsInsideOfCollider();
        Graphics.DrawMeshInstanced(mMeshFilter.sharedMesh, 0, meshMaterial, batchedMatrices, batchedMatrices.Length);
    }

}