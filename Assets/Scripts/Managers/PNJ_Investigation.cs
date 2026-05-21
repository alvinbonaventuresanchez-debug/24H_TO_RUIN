using UnityEngine;
using System.Collections;
using UnityEngine.AI;

/// <summary>
/// Gère l'investigation des salles par les NPCs
/// À mettre sur le même GameObject que PatrouilleBoucle
/// </summary>
public class PNJ_Investigation : MonoBehaviour
{
    [SerializeField] private float investigationDuration = 3f;

    private PatrouilleBoucle patrouille;
    private NavMeshAgent agent;
    private Transform joueur;
    private Coroutine investigationCoroutine;
    private string currentInvestigatingRoom;

    void Start()
    {
        patrouille = GetComponent<PatrouilleBoucle>();
        agent = GetComponent<NavMeshAgent>();

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            joueur = playerObj.transform;
    }

    /// <summary>
    /// Lance une investigation dans une salle
    /// </summary>
    public void InvestigateRoom(string roomName, ObjectMonitor targetObject)
    {
        if (RoomManager.Instance == null) return;
        if (joueur == null) return;

        // Vérifie si le joueur est dans cette salle
        string playerRoom = RoomManager.Instance.GetRoomForObject(joueur);
        if (playerRoom == roomName)
        {
            Debug.Log($"[PNJ_Investigation] Joueur détecté dans {roomName}, pas d'investigation");
            return;
        }

        if (investigationCoroutine != null)
            StopCoroutine(investigationCoroutine);

        currentInvestigatingRoom = roomName;
        investigationCoroutine = StartCoroutine(InvestigateCoroutine(roomName, targetObject));
    }

    IEnumerator InvestigateCoroutine(string roomName, ObjectMonitor targetObject)
    {
        Debug.Log($"[PNJ_Investigation] Début investigation : {roomName}");

        // Obtient le centre de la salle
        Vector3? roomCenter = RoomManager.Instance.GetRoomCenter(roomName);
        if (roomCenter == null)
            yield break;

        // Arrête la patrouille et désactive le script pour éviter les conflits
        bool patrouilleEtaitActive = patrouille != null && patrouille.enabled;
        if (patrouille != null)
        {
            patrouille.enabled = false;
        }

        // Se dirige vers la salle
        if (agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(roomCenter.Value);

            // Attend d'atteindre la salle (avec timeout de sécurité)
            float timeout = 10f;
            float elapsed = 0f;
            while (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance && elapsed < timeout)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        Debug.Log($"[PNJ_Investigation] Arrivé dans {roomName}");

        // Attend 3 secondes dans la salle
        yield return new WaitForSeconds(investigationDuration);

        Debug.Log($"[PNJ_Investigation] Investigation terminée pour {roomName}, réinitialisation de l'objet");

        // Réinitialise l'objet
        if (targetObject != null)
        {
            targetObject.ResetToInitialPosition();
            // Notifie l'objet que l'investigation est terminée (réactive le monitoring)
            targetObject.OnInvestigationComplete();
        }

        // Reprend la patrouille
        if (patrouille != null && patrouilleEtaitActive)
        {
            patrouille.ResumePatrol();
            patrouille.enabled = true;
        }

        currentInvestigatingRoom = null;
        investigationCoroutine = null;
    }

    /// <summary>
    /// Retourne si le NPC est en train d'investiguer
    /// </summary>
    public bool IsInvestigating()
    {
        return investigationCoroutine != null;
    }

    public string GetCurrentInvestigatingRoom()
    {
        return currentInvestigatingRoom;
    }
}
