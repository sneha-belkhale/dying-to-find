// MoveTo.cs
using UnityEngine;
using UnityEngine.AI;

public class AgentBrain : MonoBehaviour
{
  public Transform goal;
  private NavMeshAgent m_Agent;

  void Start()
  {
    m_Agent = GetComponent<NavMeshAgent>();
    m_Agent.destination = goal.position;
  }

  void Update()
  {
    if (Vector3.Distance(transform.position, goal.position) < 2)
    {
      Destroy(gameObject);
    }
  }
}