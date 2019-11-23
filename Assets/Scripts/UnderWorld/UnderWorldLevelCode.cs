using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderWorldLevelCode : MonoBehaviour
{
    [SerializeField] Transform startingSpawn;
    void Start()
    {
        CCPlayer.localPlayer.Teleport(startingSpawn);
    }

    void Update()
    {
        
    }
}
