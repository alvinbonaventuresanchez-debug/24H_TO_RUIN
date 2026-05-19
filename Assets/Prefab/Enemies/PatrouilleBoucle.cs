using UnityEngine;
using UnityEngine.AI;

public class PatrouilleBoucle : MonoBehaviour
{
    [Header("Waypoints dans l'ordre du tour")]
    public Transform[] waypoints;

    [Header("Distance pour passer au waypoint suivant")]
    public float seuilArrivee = 0.5f;

    private NavMeshAgent agent;
    private int indexCourant = 0;
    private bool demarre = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (waypoints.Length == 0) return;
        if (!agent.isOnNavMesh) return;

        if (!demarre)
        {
            agent.SetDestination(waypoints[indexCourant].position);
            demarre = true;
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= seuilArrivee)
        {
            PasserWaypointSuivant();
            return;
        }
    }

    void PasserWaypointSuivant()
    {
        indexCourant = (indexCourant + 1) % waypoints.Length;
        agent.SetDestination(waypoints[indexCourant].position);
    }
}
