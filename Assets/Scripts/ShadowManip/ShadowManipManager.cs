using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowManipManager : MonoBehaviour
{
    [SerializeField] Transform startingSpawn;
    void Start()
    {
        // scene setup 
        // spawn point 
        CCPlayer.main.Teleport(startingSpawn);
        CCPlayer.main.useGravity = false;

        CCPlayer.main.SetActiveHandType(HandType.Shadow);
    }

}
