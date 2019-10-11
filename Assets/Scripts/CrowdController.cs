using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CrowdController : MonoBehaviour
{
  public Transform agent;
  public float spawnInterval = 0.05f;

  void InstantiateAgent(Vector3 pos)
  {
    Transform newAgent = Instantiate(agent, pos, Quaternion.identity);

    float randomScale = Random.Range(0.8f, 1.2f);

    newAgent.localScale = new Vector3(newAgent.localScale.x * randomScale, newAgent.localScale.y * randomScale, newAgent.localScale.z * randomScale);

    float randomSpeed = Random.Range(0.4f, 0.9f);

    Animator animator = newAgent.GetComponent<Animator>();
    animator.speed = randomSpeed;

    NavMeshAgent navMeshAgent = newAgent.GetComponent<NavMeshAgent>();
    navMeshAgent.speed = randomSpeed;
    navMeshAgent.enabled = true;
  }

  private IEnumerator SpawnAgents()
  {
    while (true)
    {
      float pMin = -10.0f;
      float pMax = 10.0f;
      Vector3 pos = new Vector3(Random.Range(pMin, pMax), 0.0f, 20.0f);
      InstantiateAgent(pos);
      yield return new WaitForSeconds(spawnInterval);
    }
  }

  IEnumerator Start()
  {
    for (int i = -10; i < 10; i++)
    {
      for (int j = -10; j < 10; j++) {
        Vector3 pos = new Vector3(agent.position.x + i, agent.position.y, agent.position.z + j);
        InstantiateAgent(pos);
      }
    }
    yield return new WaitForSeconds(10.0f);
    StartCoroutine(SpawnAgents());
    print("Coroutine started");
  }
}
