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
    [SerializeField] public AudioSource FlatlineBeep;
    [SerializeField] public AudioSource RopeDrop;
    [SerializeField] public AudioSource Wind;
    [SerializeField] public AudioSource Landing;


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
        CCPlayer.main.SetActiveHandType(HandType.Voxel);
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
        Landing.Play();
        CCPlayer.main.useGravity = false;
        //fade in the beep sound
        FlatlineBeep.Play();
        float startingWindVol = Wind.volume;
        this.xuTween((float t ) => {
            FlatlineBeep.volume = 0.5f * t;
            Wind.volume = Mathf.Lerp(startingWindVol, 0, t);
        }, 4f);
        yield return new WaitForSeconds(3f);

        finalRope.gameObject.SetActive(true);
        Vector3 ropeEndPos = CCPlayer.main.head.transform.position + 2f * CCPlayer.main.head.forward.withY(0);
        finalRope.position = ropeEndPos.withY(CCPlayer.main.head.position.y - 0.75f);
        finalRope.rotation = Quaternion.LookRotation(CCPlayer.main.head.transform.forward.withY(0).normalized);
        RopeDrop.Play();

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
        ShaderEnvProps.instance.RecordGlobalPulse(CCPlayer.main.head.position);

        SuckInSound.pitch = 0.33f;
        SuckInSound.Play();
        this.xuTween((float t) => {
            float cubicT = Easing.Cubic.In(t); 
            finalVoxel.transform.position = startingVoxelPos.withY(startingVoxelPos.y + cubicT * 2f) + 0.3f * Vector3.right * Mathf.Sin(4f * Time.fixedTime);
        }, 4f);
        yield return new WaitForSeconds(0.3f);        

        ShaderEnvProps.instance.RecordGlobalPulse(CCPlayer.main.head.position);
        
        yield return new WaitForSeconds(0.8f);
        ShaderEnvProps.instance.RecordGlobalPulse(CCPlayer.main.head.position);
        yield return new WaitForSeconds(1.9f);

        this.xuTween((float t) => {
            float quinticT = Easing.Quintic.In(t);
            RenderSettings.fogColor = quinticT * Color.white;
            RenderSettings.fogDensity = 0.003f + quinticT;
        }, 1f);        
        yield return new WaitForSeconds(1f);
        CCSceneUtils.instance.StartCoroutine(CCSceneUtils.DoFadeSceneLoadCoroutine("ShadowManipScene", "UnderWorldScene"));
    }
}
