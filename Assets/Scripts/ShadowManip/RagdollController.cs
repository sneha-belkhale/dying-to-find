using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    struct SavedTransform 
    {
        public Vector3 position;
        public Quaternion rotation;
    }
    bool isKinematic = true;
    [SerializeField] Animator animator;
    [SerializeField] Transform lightPos;

    
    SavedTransform[] lastAnimatedTransforms;
    SavedTransform[] lastRbTransforms;
    Rigidbody[] rbs;

    int ragdollLayerMask;
    int shadowCastLayerMask;
    void Start()
    {
        // initialize
        rbs = GetComponentsInChildren<Rigidbody>();
        lastAnimatedTransforms = new SavedTransform[rbs.Length];
        lastRbTransforms = new SavedTransform[rbs.Length];
        SnapshotTransforms(lastAnimatedTransforms);

        // initialize in kinematic state
        animator.enabled = true;
        SetRagdollKinematic(true);

        ragdollLayerMask = 1 << 9;
        shadowCastLayerMask = 1 << 10;
    }

    void SetRagdollKinematic(bool kinematic)
    {
        foreach (var rb in rbs)
        {
            rb.isKinematic = kinematic;
        }
    }
    void SnapshotTransforms(SavedTransform[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i].position = rbs[i].transform.position;
            arr[i].rotation = rbs[i].transform.rotation;
        }
    }
    Rigidbody rbThatFollows;
    Vector3 rbTargetPoint;
    CCHand activeHand;
    void Update()
    {
        // temp  mouse raycaster
        RaycastHit hit;
        bool leftHandDown = CCPlayer.main.leftHand.handInput.triggerState == InputState.Down;
        bool rightHandDown = CCPlayer.main.rightHand.handInput.triggerState == InputState.Down;
        if((leftHandDown || rightHandDown)
         && animator.enabled)
        {
            CCHand tActiveHand = leftHandDown ? CCPlayer.main.leftHand : CCPlayer.main.rightHand;
            if (Physics.Raycast(lightPos.position, (tActiveHand.anchor.position - lightPos.position).normalized, out hit, 100, ragdollLayerMask)) {
                Debug.Log(hit.collider.name);
                if(hit.collider.name.StartsWith("mixamorig")){
                    // make this collider kinematic
                    TransitionToRagdoll();
                    rbThatFollows = hit.rigidbody;
                    rbThatFollows.isKinematic = true;
                    activeHand = tActiveHand;
                }
            }
        }
        if(activeHand && activeHand.handInput.triggerState == InputState.Released)
        {
            activeHand = null;
            rbThatFollows = null;
            StartCoroutine(TransitionToAnimator());
        }

        if(rbThatFollows != null)
        {
            bool wasHit = Physics.Raycast(lightPos.position, (activeHand.anchor.position - lightPos.position).normalized, out hit, 100, shadowCastLayerMask);
            if(wasHit && hit.collider.name.StartsWith("MouseIntersectionPlane"))
            {
                rbTargetPoint = Vector3.Lerp(rbTargetPoint, hit.point, 3f * Time.deltaTime);
                rbThatFollows.MovePosition(hit.point);
            }
        }
    }
    // set kinematic first, then lerp bones. 
    IEnumerator TransitionToAnimator()
    {
        SetRagdollKinematic(true);
        SnapshotTransforms(lastRbTransforms);

        yield return this.xuTween((float t) => {
            float easeT = Easing.Quintic.InOut(t);
            for (int i = 0; i < lastAnimatedTransforms.Length; i++)
            {
                rbs[i].transform.position = Vector3.Lerp(lastRbTransforms[i].position, lastAnimatedTransforms[i].position, easeT);
                rbs[i].transform.rotation = Quaternion.Lerp(lastRbTransforms[i].rotation, lastAnimatedTransforms[i].rotation, easeT);
            }
        }, 1f);
        animator.enabled = true;
        yield return 0f;
    }
    //lerp out bones then set non kinematic
    public void TransitionToRagdoll()
    {
        animator.enabled = false;
        SetRagdollKinematic(false);
    }

    
}
