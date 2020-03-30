using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowManipManager : MonoBehaviour
{
    [SerializeField] Transform startingSpawn;
    [SerializeField] public Transform spotlight;

    static public ShadowManipManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
            instance = this;
        }
    }

    void Start()
    {
        // scene setup 
        CCPlayer.main.Teleport(startingSpawn);
        CCPlayer.main.useGravity = false;

        CCPlayer.main.SetActiveHandType(HandType.Shadow);
    }

    void Update()
    {
        spotlight.position = CCPlayer.main.leftHand.transform.position;
        spotlight.rotation = CCPlayer.main.leftHand.transform.rotation;
    }

}
