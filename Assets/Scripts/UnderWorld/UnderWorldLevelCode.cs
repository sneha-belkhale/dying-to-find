using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderWorldLevelCode : MonoBehaviour
{
    [SerializeField] Transform startingSpawn;
    float pAcc;
    float pVelo;
    bool falling = false;
    void Start()
    {
        CCPlayer.localPlayer.Teleport(startingSpawn);
        CCPlayer.localPlayer.antiGravity = true;
        ResetAcceleration();
    }

    void ResetAcceleration() {
       pAcc = -9.8f;
       pVelo = 1f;
    }

    void Update()
    { 
        if(CCPlayer.localPlayer.isGrabbing){
            if(falling) {
                // reset velocity
                ResetAcceleration();
                falling = false;
            }
            return;
        } else {
            falling = true;
        }

        Vector3 pos = CCPlayer.localPlayer.transform.position;
        pVelo = Mathf.Max(pVelo + pAcc * Time.deltaTime, -4.5f);
        pos.y += pVelo * Time.deltaTime;
        CCPlayer.localPlayer.transform.position = pos;
    }
}
