// MoveTo.cs
using UnityEngine;
using UnityEngine.AI;

public class AgentBrain : MonoBehaviour
{
  [SerializeField]
  private float handCollisionRadius = 0.5f;
  private NavMeshAgent m_Agent;
  private Quaternion lastHeadRot;

  public Transform goal;
  public Transform headPos;
  public Transform playerRightHand;
  public Transform playerLeftHand;
  public Transform playerHead;

  void Start()
  {
    m_Agent = GetComponent<NavMeshAgent>();
    m_Agent.destination = goal.position;
    lastHeadRot = headPos.rotation;
  }

  void LateUpdate()
  {
    if (Vector3.Distance(transform.position, goal.position) < 2)
    {
      Destroy(gameObject);
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