using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class CrowdController : MonoBehaviour
{
  public Transform agent;
  public float spawnInterval = 0.05f;
    int pMin = -5;
    int pMax = 5;

  void InstantiateAgent(Vector3 pos, int offset)
  {
    Transform newAgent = Instantiate(agent, pos, Quaternion.identity, CCSceneUtils.instance.instantiatedObjectRoot);

    float randomScale = Random.Range(1f, 1.2f);

    newAgent.localScale = new Vector3(newAgent.localScale.x * randomScale, newAgent.localScale.y * randomScale, newAgent.localScale.z * randomScale);

    float randomSpeed = Random.Range(0.4f, 0.9f);

    Animator animator = newAgent.GetComponent<Animator>();
    animator.speed = randomSpeed;

    NavMeshAgent navMeshAgent = newAgent.GetComponent<NavMeshAgent>();
    navMeshAgent.speed = randomSpeed;
    navMeshAgent.enabled = true;

    AgentBrain brain = newAgent.GetComponent<AgentBrain>();
    brain.SetOffset(offset);

  }

  void Start()
  {
    for (int i = pMin; i < pMax; i++)
    {
      for (int j = pMin; j < pMax; j++) {
        Vector3 pos = new Vector3(agent.position.x + i, agent.position.y, agent.position.z + j);
        InstantiateAgent(pos, i * (pMax - pMin) + j);
      }
    }
  }

  void Update(){
  }
}
