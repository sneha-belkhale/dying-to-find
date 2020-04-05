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

    
    SavedTransform[] lastAnimatedTransforms;
    SavedTransform[] lastRbTransforms;
    Rigidbody[] rbs;

    int ragdollLayerMask;
    int shadowCastLayerMask;
    void Start()
    {
        // initialize
        lastTargetPoint = animator.gameObject.transform.position;
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
    CCHand activeHand;
    void Update()
    {

        Vector3 lightPos = ShadowManipManager.instance.spotlight.position;
        RaycastHit hit;
        bool rightHandDown = CCPlayer.main.rightHand.handInput.triggerState == InputState.Down;

        if((rightHandDown)
         && animator.enabled)
        {
            CCHand tActiveHand = CCPlayer.main.rightHand;
            if (Physics.Raycast(lightPos, (tActiveHand.anchor.position - lightPos).normalized, out hit, 100, ragdollLayerMask)) {
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
            rbThatFollows.isKinematic = false;
            rbThatFollows = null;
            activeHand = null;
            StartCoroutine(TransitionToAnimator());
        }

        if(rbThatFollows != null)
        {
            bool wasHit = Physics.Raycast(lightPos, (activeHand.anchor.position - lightPos).normalized, out hit, 100, shadowCastLayerMask);
            if(wasHit && hit.collider.name.StartsWith("MouseIntersectionPlane"))
            {

                rbThatFollows.MovePosition(hit.point + 1.5f * hit.collider.transform.up);
            }
        }
    }
    // set kinematic first, then lerp bones. 
    Vector3 lastTargetPoint = Vector3.zero;
    IEnumerator TransitionToAnimator()
    {
        yield return new WaitForSeconds(2f);

        SetRagdollKinematic(true);
        SnapshotTransforms(lastRbTransforms);
        
        Vector3 rbTargetPoint = rbs[0].position.withY(0f);
        Vector3 diff = (rbTargetPoint - lastTargetPoint).withY(0f); // xz plane only
        lastTargetPoint = rbTargetPoint;
        TranslateTransforms(lastAnimatedTransforms, diff);
        animator.gameObject.transform.position += diff;

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

    void TranslateTransforms(SavedTransform[] arr, Vector3 diff)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i].position += diff;
        }
    }
    //lerp out bones then set non kinematic
    public void TransitionToRagdoll()
    {
        animator.enabled = false;
        SetRagdollKinematic(false);
    }

    
}
