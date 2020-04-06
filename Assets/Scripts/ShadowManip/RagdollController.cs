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
    [SerializeField] BoxCollider ringTarget;
    [SerializeField] SkinnedMeshRenderer renderer;
    SavedTransform[] lastAnimatedTransforms;
    SavedTransform[] lastRbTransforms;
    Rigidbody[] rbs;

    int ragdollLayerMask;
    int shadowCastLayerMask;
    bool onConveyor = false;
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
        if(onConveyor) return;

        if(activeHand && activeHand.handInput.triggerState == InputState.Released)
        {
            ResetToAnimator();
        }

        Vector3 lightPos = ShadowManipManager.instance.spotlight.position;
        RaycastHit hit;
        if(rbThatFollows != null)
        {
            bool wasHit = Physics.Raycast(lightPos, (activeHand.anchor.position - lightPos).normalized, out hit, 100, shadowCastLayerMask);
            if(wasHit && hit.collider.name.StartsWith("MouseIntersectionPlane"))
            {
                rbThatFollows.MovePosition(hit.point + hit.collider.transform.up);
                if(ringTarget.bounds.Contains(rbThatFollows.transform.position))
                {
                    onConveyor = true;
                    StartCoroutine(OntoConveyorRoutine());
                }
            } else {
                // drop 
                ResetToAnimator();
            }
        }
    }

    public void InitiateGrab(CCHand hand, Rigidbody rb)
    {
        if(animator.enabled)
        {
            TransitionToRagdoll();
            rbThatFollows = rb;
            rbThatFollows.isKinematic = true;
            activeHand = hand;
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
        ClampToCircle(ref rbTargetPoint);

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
            renderer.material.SetFloat("_FadeOutVal", 1f - easeT);
        }, 1f);
        animator.enabled = true;
        yield return 0f;
    }

    void ResetToAnimator()
    {
        rbThatFollows.isKinematic = false;
        rbThatFollows = null;
        activeHand = null;
        StartCoroutine(TransitionToAnimator());
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

    IEnumerator OntoConveyorRoutine()
    {
        SetRagdollKinematic(true);
        Vector3 startingPos = animator.transform.position;
        Vector3 startingRingPos = ringTarget.transform.position;
        float startingVel = 15f;
        while(startingVel > 1f)
        {
            startingVel = (1f -  3.7f * Time.deltaTime) * startingVel;
            Vector3 pos = animator.transform.position + Time.deltaTime * startingVel * Vector3.up;
            pos.x = Mathf.MoveTowards(pos.x, startingRingPos.x, Time.deltaTime);
            pos.z = Mathf.MoveTowards(pos.z, startingRingPos.z, Time.deltaTime);
            animator.transform.position = pos;
            yield return 0;
        };
        TransitionToRagdoll();
        animator.transform.SetParent(ringTarget.transform);
    }

    void ClampToCircle(ref Vector3 v)
    {
        v = v.normalized * (ShadowManipManager.instance.radius - 1f);
    }
}
