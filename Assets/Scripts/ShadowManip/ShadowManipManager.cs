using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowManipManager : MonoBehaviour
{
    [SerializeField] Transform startingSpawn;
    [SerializeField] public Transform spotlight;

    static public ShadowManipManager instance;

    [SerializeField] GameObject levelingPlanePrefab;
    [SerializeField] GameObject conveyorRagdollPrefab;
    [SerializeField] int numPlanes = 30;
    [SerializeField] float bounds = 5;
    [SerializeField] float offset = 5;
    [SerializeField] float radius = 8;
    
    struct LevelingPlane
    {
        public Transform t;
        public int dir; 
    }
    LevelingPlane[] levelingPlanes;

    Transform[] conveyorRagdolls;

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


        SetupLevelingPlanes();
        
        SetUpConveyor();
    }

    void Update()
    {
        spotlight.position = CCPlayer.main.leftHand.transform.position;
        spotlight.rotation = CCPlayer.main.leftHand.transform.rotation;
        for (int i = 0; i < numPlanes; i++)
        {
            Vector3 pos = levelingPlanes[i].t.position;
            pos.y += levelingPlanes[i].dir * Time.deltaTime;
            if(pos.y < -bounds - offset) {
                levelingPlanes[i].dir = 1;
            }
            if(pos.y > bounds - offset) {
                levelingPlanes[i].dir = -1;
            }
            levelingPlanes[i].t.position = pos;
        }
        UpdateConveyor();
    }

    void SetupLevelingPlanes()
    {
        levelingPlanes = new LevelingPlane[numPlanes];
        for (int i = 0; i < numPlanes ; i++)
        {
            float degree = i * (360f / numPlanes);
            Quaternion rot = Quaternion.Euler(180f, degree, 90f);
            GameObject newPlane = Instantiate(levelingPlanePrefab, Vector3.zero, rot, transform);
            newPlane.SetActive(true);
            newPlane.transform.position -= radius * newPlane.transform.up + (offset + Random.Range(-bounds, bounds)) * Vector3.up;
            levelingPlanes[i].t = newPlane.transform;
            levelingPlanes[i].dir = 2 * Random.Range(0, 2) - 1;
        }
    }


    void SetUpConveyor()
    {
        conveyorRagdolls = new Transform[10];
        for (int i = 0; i < 10 ; i++)
        {
            float degree = i * (360f / numPlanes);
            Quaternion rot = Quaternion.Euler(180f, degree, 90f);
            GameObject newConveryor = Instantiate(conveyorRagdollPrefab, Vector3.zero, rot, transform);
            newConveryor.SetActive(true);
            newConveryor.transform.position -= (radius * 1.2f) * newConveryor.transform.up - 10.3f * Vector3.up;
            conveyorRagdolls[i] = newConveryor.transform;
        }
    }

    void UpdateConveyor()
    {
    }
}
