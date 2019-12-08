// MoveTo.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentBrain : GrabbableObject
{
    [SerializeField]
    private float handCollisionRadius = 0.5f;
    private NavMeshAgent agent;
    private Quaternion lastHeadRot;
    private bool transitioning;
    [SerializeField]
    private Transform goal;
    [SerializeField]
    private Transform spawn;
    [SerializeField]
    private Transform headPos;

    private List<Transform> spawnPoints;

    void Start()
    {
        // Store a list of spawn points
        spawnPoints = new List<Transform>();
        foreach (Transform t in spawn)
        {
            spawnPoints.Add(t);
        }

        mat = GetComponentInChildren<SkinnedMeshRenderer>().material;
        mat.SetColor("_Color", Random.Range(0f, 0.2f) * Color.gray);
        agent = GetComponent<NavMeshAgent>();

        agent.Warp(transform.position);
        agent.destination = goal.position;

        lastHeadRot = headPos.rotation;

        float rSpeed = Random.Range(0.4f, 0.9f);

        // Randomize moving speed and walking animation
        Animator animator = GetComponent<Animator>();
        animator.speed = rSpeed;

        NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = rSpeed;
        navMeshAgent.enabled = true;
    }

    public void SetOffset(float offset)
    {
        Material mat = GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial;
        mat.SetFloat("_Offset", offset);
        if (offset % 4 == 0)
        {
            mat.SetFloat("_IgnoreGlobalStretchFrag", 0);
        }
        else
        {
            mat.SetFloat("_IgnoreGlobalStretchFrag", 1);
        }
    }

    IEnumerator FadeOut()
    {
        float t = 0;
        float speed = Random.Range(0.8f, 1.4f);
        Vector4 dir = new Vector4(0, 0, 1, 0);
        while (t <= 1f)
        {
            t += speed * Time.deltaTime;
            float easeT = Easing.Circular.In(t);
            dir.w = Mathf.Min(easeT, 1);
            mat.SetFloat("_FadeOutVal", Mathf.Min(easeT, 1));
            mat.SetVector("_VertOutVal", dir);
            yield return 0;
        }
        yield return 0;
    }

    IEnumerator FadeIn()
    {
        float t = 1;
        float speed = Random.Range(0.8f, 1.4f);
        Vector4 dir = new Vector4(0, 0, -1, 0);
        while (t >= 0f)
        {
            t -= speed * Time.deltaTime;
            float easeT = Easing.Circular.Out(t);
            dir.w = Mathf.Min(easeT, 1);
            mat.SetFloat("_FadeOutVal", Mathf.Max(easeT, 0));
            mat.SetVector("_VertOutVal", dir);
            yield return 0;
        }
        yield return 0;
    }

    IEnumerator DoTransition()
    {
        yield return StartCoroutine("FadeOut");

        // Select random spawn point from the list
        int spawnIndex = Random.Range(0, spawnPoints.Count);
        Transform curSpawnPoint = spawnPoints[spawnIndex];

        float rRadius = 3;

        Vector3 spawnPos = new Vector3(
            curSpawnPoint.position.x + Random.Range(-rRadius, rRadius),
            curSpawnPoint.position.y,
            curSpawnPoint.position.z + Random.Range(-rRadius, rRadius)
        );

        agent.Warp(spawnPos);
        agent.destination = goal.position;

        agent.speed = Random.Range(0.4f, 0.9f);
        yield return StartCoroutine("FadeIn");
        transitioning = false;
    }

    Coroutine glitchCoroutine = null;
    void LateUpdate()
    {
        if (Vector3.Distance(transform.position, goal.position) < 8 && !transitioning)
        {
            StartCoroutine("DoTransition");
            transitioning = true;
        }

        if ((CCPlayer.main.rightHand.transform.position - headPos.position).magnitude < handCollisionRadius ||
           (CCPlayer.main.leftHand.transform.position - headPos.position).magnitude < handCollisionRadius)
        {
            lookAt();
        }
        else
        {
            lookBack();
        }

        if (captured)
        {
            transform.position = lastAgentPos;
        }

    }

    bool captured = false;
    Vector3 lastAgentPos;
    public override void onDown()
    {
        captured = true;
        GetComponent<Animator>().SetTrigger("next");
        lastAgentPos = transform.position;
    }

    public override void onRelease()
    {
        GetComponent<Animator>().SetTrigger("next");
        captured = false;
    }

    private void lookAt()
    {
        Quaternion targetQuat = getRotationTowardsClamped(headPos, CCPlayer.main.head.transform.position, 90f);
        headPos.rotation = Quaternion.Lerp(lastHeadRot, targetQuat, 2f * Time.deltaTime);
        lastHeadRot = headPos.rotation;
    }

    private void lookBack()
    {
        headPos.rotation = Quaternion.Lerp(lastHeadRot, headPos.rotation, 3f * Time.deltaTime);
        lastHeadRot = headPos.rotation;
    }

    Quaternion getRotationTowardsClamped(Transform current, Vector3 targetPos, float constraint)
    {
        Vector3 targetDir = (targetPos - current.position).normalized;

        float angle = Vector3.SignedAngle(targetDir.withY(0f), current.forward.withY(0f), Vector3.up);
        angle = Mathf.Clamp(angle, -constraint, constraint);

        return Quaternion.AngleAxis(-angle, Vector3.up) * current.rotation;
    }
}
