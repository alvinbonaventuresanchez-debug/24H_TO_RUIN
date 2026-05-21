using UnityEngine;
using UnityEngine.AI;

public class PatrouilleBoucle : MonoBehaviour
{
    [Header("Waypoints dans l'ordre du tour")]
    public Transform[] waypoints;

    [Header("Distance pour passer au waypoint suivant")]
    public float seuilArrivee = 0.5f;

    [Header("Attraction joueur")]
    public Transform joueur;
    public float seuilArriveeJoueur = 1f;

    private NavMeshAgent agent;
    private int indexCourant = 0;
    private bool demarre = false;
    private bool attireParJoueur = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null && joueur == null)
            joueur = playerObj.transform;
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

        // Mode attraction : suit le joueur en continu
        if (attireParJoueur)
        {
            if (joueur != null)
                agent.SetDestination(joueur.position);
            return;
        }

        // Patrouille normale
        if (!agent.pathPending && agent.remainingDistance <= seuilArrivee)
        {
            PasserWaypointSuivant();
        }
    }

    public void AttirerVersJoueur(bool actif)
    {
        if (joueur == null) return;
        attireParJoueur = actif;

        if (!actif)
            PasserWaypointSuivant();
    }

    void PasserWaypointSuivant()
    {
        indexCourant = (indexCourant + 1) % waypoints.Length;
        agent.SetDestination(waypoints[indexCourant].position);
    }

    /// <summary>
    /// Arrête la patrouille automatique (appelé par PNJ_Investigation)
    /// </summary>
    public void StopPatrol()
    {
        demarre = false;
        if (agent != null)
            agent.ResetPath();
    }

    /// <summary>
    /// Reprend la patrouille (appelé par PNJ_Investigation)
    /// </summary>
    public void ResumePatrol()
    {
        demarre = false;
    }
}
