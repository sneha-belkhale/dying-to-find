// MoveTo.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentBrain : MonoBehaviour
{
  [SerializeField]
  private float handCollisionRadius = 0.5f;
  private NavMeshAgent m_Agent;
  private Material mat;
  private Quaternion lastHeadRot;
  private bool transitioning;

  public Transform goal;
  public Transform headPos;
  public Transform playerRightHand;
  public Transform playerLeftHand;
  public Transform playerHead;

  void Start()
  {
    mat = GetComponentInChildren<SkinnedMeshRenderer>().material;
    m_Agent = GetComponent<NavMeshAgent>();
    m_Agent.destination = goal.position;
    lastHeadRot = headPos.rotation;
  }

  IEnumerator FadeOut() {
    float t = 0;
    float speed = Random.Range(0.8f, 1.4f);
    Vector4 dir = new Vector4(0,0,1,0);
    while(t <= 1f){
      t += speed * Time.deltaTime;
      float easeT = Easing.Circular.In(t);
      dir.w = Mathf.Min(easeT, 1);
      mat.SetFloat("_FadeOutVal", Mathf.Min(easeT, 1));
      mat.SetVector("_VertOutVal", dir);
      yield return 0;
    }
    yield return 0;
  }

  IEnumerator FadeIn() {
    float t = 1;
    float speed = Random.Range(0.8f, 1.4f);
    Vector4 dir = new Vector4(0,0,-1,0);
    while(t >= 0f){
      t -= speed * Time.deltaTime;
      float easeT = Easing.Circular.Out(t);
      dir.w = Mathf.Min(easeT, 1);
      mat.SetFloat("_FadeOutVal", Mathf.Max(easeT,0));
      mat.SetVector("_VertOutVal", dir);
      yield return 0;
    }
    yield return 0;
  }

  IEnumerator DoTransition() {
    yield return StartCoroutine("FadeOut");
    Vector3 pos = new Vector3(Random.Range(-5, 5), 0.0f, 20.0f);
    transform.position = pos;
    m_Agent.speed = Random.Range(0.4f, 0.9f);
    yield return StartCoroutine("FadeIn");
    transitioning = false;

  }
  void LateUpdate()
  {
    if (Vector3.Distance(transform.position, goal.position) < 8 && !transitioning)
    {
      StartCoroutine("DoTransition");
      transitioning = true;
    }

    if((playerRightHand.position - headPos.position).magnitude < handCollisionRadius ||
       (playerLeftHand.position - headPos.position).magnitude < handCollisionRadius)
    {
      lookAt();
    } else {
      lookBack();
    }
  }

  private void lookAt()
  {
    Vector3 dir = (playerHead.position - headPos.position).normalized;
    Quaternion newHeadRot = Quaternion.LookRotation(dir);

    headPos.rotation = Quaternion.Lerp(lastHeadRot, newHeadRot, 2f * Time.deltaTime);
    lastHeadRot = headPos.rotation;
  }

  private void lookBack()
  {
    headPos.rotation = Quaternion.Lerp(lastHeadRot, headPos.rotation, 3f * Time.deltaTime);
    lastHeadRot = headPos.rotation;
  }
}
