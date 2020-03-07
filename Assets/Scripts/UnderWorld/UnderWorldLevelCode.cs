using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderWorldLevelCode : MonoBehaviour
{
    [SerializeField] Transform startingSpawn;
    [SerializeField] SplineSpawner splineSpawner;
    public static UnderWorldLevelCode instance;
    Coroutine dropPlayerRope = null;
    [SerializeField] Transform finalRope;
    [SerializeField] Transform finalRopeEnd;
    [SerializeField] Transform finalPlane;
    public bool LevelCompleted = false;
    float finalLandingY;
    private int _numPlatformsCompleted = 0;
    public int NumPlatformsCompleted {
        get {
            return _numPlatformsCompleted;
        }
        set {
            _numPlatformsCompleted = value;
            if(_numPlatformsCompleted > 3 && !LevelCompleted)
            {
                LevelCompleted = true;
                // set the final landing position;
                finalLandingY = -152f * (splineSpawner.lastChunkPos + 2) - 30f;
                // drop a plane
                finalPlane.gameObject.SetActive(true);
                finalPlane.position = new Vector3(CCPlayer.main.transform.position.x, finalLandingY, CCPlayer.main.transform.position.z);
            }
        }
    }
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
        CCPlayer.main.Teleport(startingSpawn);
        CCPlayer.main.useGravity = true;
        CCPlayer.main.ResetAcceleration();

        RenderSettings.fog = true;
        RenderSettings.fogColor = Color.black;
        RenderSettings.fogDensity = 0.15f;
    }

    void Update()
    {
        if(!LevelCompleted) return;

        // wait until player has reached the end of the last spline env
        if(CCPlayer.main.transform.position.y < finalLandingY && dropPlayerRope == null){
            // player's env should now be empty
            // stop player from falling and drop a platform 
            CCPlayer.main.useGravity = false;
            dropPlayerRope = StartCoroutine(DropPlayerRope());
        }
    }

    IEnumerator DropPlayerRope()
    {
        finalRope.gameObject.SetActive(true);
        yield return this.xuTween((t) => {
            finalRope.position = new Vector3(CCPlayer.main.transform.position.x + 1f,CCPlayer.main.head.position.y + 5f * (1f - t),CCPlayer.main.transform.position.z);
        }, 2f);
        while(Vector3.SqrMagnitude(finalRopeEnd.position - CCPlayer.main.head.position) > 0.5f)
        {
            yield return 0;
        }
        //TODO symbolize that player is glitching out somehow.
        CCSceneUtils.instance.StartCoroutine(CCSceneUtils.DoFadeSceneLoadCoroutine("RopeScene", "UnderWorldScene"));
    }
}
