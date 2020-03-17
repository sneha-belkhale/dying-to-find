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
    [SerializeField] Transform finalVoxel;
    [SerializeField] SoundArea ambientSoundArea;
    public bool LevelCompleted = false;
    float finalLandingY;
    private int _numPlatformsCompleted = 0;
    public int NumPlatformsCompleted {
        get {
            return _numPlatformsCompleted;
        }
        set {
            _numPlatformsCompleted = value;
            if(_numPlatformsCompleted > 2 && !LevelCompleted)
            {
                LevelCompleted = true;
                splineSpawner.RemoveSplinesFromPos(CCPlayer.main.transform.position.y, 35f);
                // set the final landing position;
                finalLandingY = CCPlayer.main.transform.position.y - 95f;
                // drop a plane
                finalPlane.gameObject.SetActive(true);
                ambientSoundArea.StartFadeOut();
            }
        }
    }

    //AUDIO static references
    [SerializeField] public AudioSource SuckInSound;
    [SerializeField] public AudioSource IdleVoxelSound;
    [SerializeField] public AudioSource GrabSound;
    [SerializeField] public AudioSource PulseSound;


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
        if(Input.GetKeyDown("c") || OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.LTouch)) {
            NumPlatformsCompleted ++;
        }
        if(!LevelCompleted) return;

        // wait until player has reached the end of the last spline env
        if(CCPlayer.main.transform.position.y < (finalLandingY + 1.5f) && dropPlayerRope == null){
            // player's env should now be empty
            // stop player from falling and drop a platform 
            dropPlayerRope = StartCoroutine(EndScene());
        } else if (dropPlayerRope == null) {
            // make sure plane is always under character
            finalPlane.position = new Vector3(CCPlayer.main.transform.position.x, finalLandingY, CCPlayer.main.transform.position.z);
        }
    }

    IEnumerator EndScene()
    {
        CCPlayer.main.useGravity = false;
        yield return new WaitForSeconds(3f);

        finalRope.gameObject.SetActive(true);
        Vector3 ropeEndPos = CCPlayer.main.head.transform.position + 2.5f * CCPlayer.main.head.forward.withY(0);
        finalRope.position = ropeEndPos.withY(CCPlayer.main.head.position.y - 0.5f);
        yield return new WaitForSeconds(7f);    

        // wait until they grabbed the object atleast once
        StationaryGrabbableObject g = finalRopeEnd.GetComponent<StationaryGrabbableObject>();
        while(g.grabber[0] ==null && g.grabber[1] == null)
        {
            yield return 0f;
        }
        while(Vector3.SqrMagnitude(finalRopeEnd.position - CCPlayer.main.head.position) > 0.25f)
        {
            yield return 0;
        }
        
        Vector3 startingVoxelPos = CCPlayer.main.head.position.withY(CCPlayer.main.head.position.y - 1f);
        finalVoxel.gameObject.SetActive(true);
        yield return this.xuTween((float t) => {
            finalVoxel.transform.position = startingVoxelPos.withY(startingVoxelPos.y + t * 2f) + 0.3f * Vector3.right * Mathf.Sin(4f * Time.fixedTime);
        }, 3f);
        
        //TODO symbolize that player is glitching out somehow.
        CCSceneUtils.instance.StartCoroutine(CCSceneUtils.DoFadeSceneLoadCoroutine("RopeScene", "UnderWorldScene"));
    }
}
