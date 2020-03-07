using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class InstancedVoxelMesh : MonoBehaviour
{
    public GameObject prefab;
    public Material meshMaterial;
    public float gridSize;
    public float timeToLive;
    public float spacing;
    private MeshFilter mMeshFilter;
    private MeshRenderer mMeshRenderer;
    private Dictionary<Vector3, Vector2> voxelGrid;
    private Matrix4x4[] matrices;
    public BoxCollider[] colliders;
    float lastY;

    // Utils
    public void Round(ref Vector3 v)
    {
        v.x = Mathf.Round(v.x);
        v.y = Mathf.Round(v.y);
        v.z = Mathf.Round(v.z);
    }
    public Matrix4x4 GetUnscaledMatrix(Transform transform)
    {
    return Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
    }
    void Start ()
    {
        mMeshFilter = prefab.GetComponent<MeshFilter>();
        mMeshRenderer = prefab.GetComponent<MeshRenderer>();
        voxelGrid = new Dictionary<Vector3, Vector2>();
    }
    private Matrix4x4[] GetPointsInsideOfCollider()
    {
        float moveDif = Mathf.Clamp(Mathf.Abs(lastY  - transform.position.y)/.1f, 0.1f, 1f);
        Vector3 scale = spacing * gridSize * new Vector3(1, 1, 1);

        for (int col = 0; col < colliders.Length; col++)
        {
            BoxCollider bCollider = colliders[col];
            int depth = Mathf.CeilToInt(bCollider.gameObject.transform.lossyScale.z * bCollider.size.z/gridSize);
            int height = Mathf.CeilToInt(bCollider.gameObject.transform.lossyScale.y *  bCollider.size.y/gridSize);
            int width = Mathf.CeilToInt(bCollider.gameObject.transform.lossyScale.x *  bCollider.size.x/gridSize);
            
            Vector3 starting = -Vector3.Scale(bCollider.transform.lossyScale, 0.5f * bCollider.size);
            Matrix4x4 unscaledMatrix = GetUnscaledMatrix(bCollider.gameObject.transform);
            for (int i = 1; i < width; ++i)
            {
                for (int j = 1; j < height; ++j)
                {
                    for (int k = 1; k < depth; k ++) 
                    {
                        Vector3 pos = starting + new Vector3(i * gridSize, j * gridSize, k*gridSize);
                        pos = unscaledMatrix.MultiplyPoint3x4(pos);
                        pos /= gridSize;
                        Round(ref pos);
                        pos *= gridSize; 
                        if(!voxelGrid.ContainsKey(pos))
                        {
                            voxelGrid.Add(pos, new Vector2());
                        }
                        Vector2 val = voxelGrid[pos];
                        val.x = Mathf.MoveTowards(val.x, timeToLive, moveDif * timeToLive);
                        val.y = 1f;
                        voxelGrid[pos] = val;
                    }
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
                val.x = Mathf.MoveTowards(val.x, 0f, 0.2f * moveDif * timeToLive);
            }
            val.y = 0f;
            if(val.x <= moveDif*0.04f)  
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
        lastY = transform.position.y;
    }

}