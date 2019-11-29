using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderWorldLevelCode : MonoBehaviour
{
    [SerializeField] Transform startingSpawn;
    void Start()
    {

        CCPlayer.main.Teleport(startingSpawn);
        CCPlayer.main.antiGravity = true;
        CCPlayer.main.ResetAcceleration();

        RenderSettings.fog = true;
        RenderSettings.fogColor = Color.black;
        RenderSettings.fogDensity = 0.15f;
    }

    void Update()
    {
    }
}
