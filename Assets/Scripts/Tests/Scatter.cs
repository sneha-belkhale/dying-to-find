using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scatter : MonoBehaviour {
    public GameObject source;
    public Material meshMaterial;

    private Matrix4x4[] batchedMatrices;
    private MeshFilter meshFilter;

    Matrix4x4[] BuildParticlePositions (int amount, float width, float height) {
        Matrix4x4[] matrices = new Matrix4x4[amount * amount * amount];
        Vector3 nFreq = new Vector3 (1.0f, 1.0f, 1.0f);
        float nAmp = 5.0f;
        int count = 0;

        for (int i = 0; i < amount; i++) {
            for (int j = 0; j < amount; j++) {
                for (int k = 0; k < amount; k++) {

                    float nX = (i / (float) amount) * nFreq.x;
                    float nY = (j / (float) amount) * nFreq.y;
                    float nZ = (k / (float) amount) * nFreq.z;

                    float n = Perlin.Noise (nX, nY, nZ) * nAmp;

                    Vector3 pos = new Vector3 (n + i, n + j, n + k);
                    Vector3 scale = new Vector3 (0.2f, 0.2f, 0.2f);

                    Matrix4x4 m = Matrix4x4.identity;

                    m.SetTRS (pos, Quaternion.identity, scale);

                    matrices[count] = m;
                    count++;
                }
            }

        }

        return matrices;
    }

    void Start () {
        int amountToScatter = 10;
        meshFilter = source.GetComponent<MeshFilter> ();
        batchedMatrices = BuildParticlePositions (amountToScatter, 10.0f, 10.0f);
    }

    void Update () {
        Graphics.DrawMeshInstanced (meshFilter.sharedMesh, 0, meshMaterial, batchedMatrices, batchedMatrices.Length);
    }
}