using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshTest : MonoBehaviour
{
    NavMeshAgent _agent;
    public float wanderRadius = 10;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();


        // Start the coroutine to make the entity move randomly
        // StartCoroutine(Wander());
    }

    void Update()
    {
        if (_agent.remainingDistance < 0.5f)
        {
            Vector3 randomPoint = Random.insideUnitSphere * wanderRadius;
            randomPoint += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, wanderRadius, 1))
            {
                _agent.SetDestination(hit.position);
            }
        }
        else if (_agent.hasPath)
        {
            var transform1 = transform;
            Vector3 toTarget = _agent.steeringTarget - transform1.position;
            float turnAngle = Vector3.Angle(transform1.forward, toTarget);
            _agent.acceleration = turnAngle;
        }
    }

    IEnumerator Wander()
    {
        while (true)
        {
            // Calculate a random point in the wander radius around your entity
            Vector3 randomPoint = Random.insideUnitSphere * wanderRadius;
            randomPoint += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, wanderRadius, 1))
            {
                // If a valid NavMesh position is found in the wander radius, the entity will move there
                _agent.SetDestination(hit.position);

                // Wait until the entity almost reaches its destination, then continue with the next iteration of the loop
                while (_agent.remainingDistance > _agent.stoppingDistance || _agent.pathPending)
                    yield return null;
            }
            else
            {
                // If a valid position is not found, wait a frame then try again
                yield return null;
            }
        }
    }
}