using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderWorldLevelCode : MonoBehaviour
{
    [SerializeField] Transform startingSpawn;
    void Start()
    {
        CCPlayer.localPlayer.Teleport(startingSpawn);
        CCPlayer.localPlayer.antiGravity = true;

        RenderSettings.fogColor = Color.black;
        RenderSettings.fogDensity = 0.015f;

    }

    void Update()
    { 
    }
}
