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
    }

    void Update()
    { 
    //     if(CCPlayer.localPlayer.isGrabbing) return;
    //     // only generate new world points if falling
    //     Vector3 position = CCPlayer.localPlayer.transform.position;
    //     position.y -= Time.deltaTime;
    //     CCPlayer.localPlayer.transform.position = position;
    }
}
