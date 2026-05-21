using UnityEngine;
using System.Collections;

/// <summary>
/// Monitor les objets pour détecter s'ils ont bougé de leur position initiale
/// À ajouter à chaque objet portable
/// </summary>
public class ObjectMonitor : MonoBehaviour
{
    [SerializeField] private float detectionDelay = 1.5f;
    [SerializeField] private float positionTolerance = 0.5f;  // Augmenté pour éviter gravity detection

    private Vector3 positionInitiale;
    private string roomInitiale;
    private bool hasBeenMoved = false;
    private bool investigationInProgress = false;  // Bloque les investigations en boucle
    private Coroutine investigationCoroutine;
    private bool wasTakenByPlayer = false;  // Track si l'objet a été pris par le joueur

    void Awake()
    {
        // Sauvegarde la position initiale AU DÉMARRAGE (Awake, pas Start)
        positionInitiale = transform.position;
    }

    void Start()
    {
        // Attend que RoomManager soit initialisé
        if (RoomManager.Instance != null)
        {
            roomInitiale = RoomManager.Instance.GetRoomForObject(transform);
            Debug.Log($"[ObjectMonitor] {gameObject.name} initialized in room: {roomInitiale} at position {positionInitiale}");
        }
        else
        {
            Debug.LogWarning($"[ObjectMonitor] RoomManager not found for {gameObject.name}");
            roomInitiale = "Unknown";
        }
    }

    void Update()
    {
        if (RoomManager.Instance == null) return;

        // Si l'objet est tenu par le joueur, désactiver le monitoring
        if (transform.parent != null)
        {
            wasTakenByPlayer = true;
            // L'objet est enfant de quelque chose (probablement le joueur)
            // Réinitialiser les flags si l'était en cours d'investigation
            if (investigationCoroutine != null)
            {
                StopCoroutine(investigationCoroutine);
                investigationCoroutine = null;
            }
            // L'objet sera re-monitoré quand il sera lâché (parent = null)
            return;
        }

        // Détection: l'objet vient d'être lâché
        if (wasTakenByPlayer && !investigationInProgress)
        {
            Debug.Log($"[ObjectMonitor] {gameObject.name} a été lâché, mise à jour position de repos");
            positionInitiale = transform.position;
            roomInitiale = RoomManager.Instance.GetRoomForObject(transform);
            wasTakenByPlayer = false;
            hasBeenMoved = false;  // Réinitialiser pour permettre une nouvelle détection
            return;  // Attendre la prochaine frame pour détecter le mouvement
        }

        // Vérifie si l'objet a bougé
        float distanceDeplacee = Vector3.Distance(transform.position, positionInitiale);
        string roomActuelle = RoomManager.Instance.GetRoomForObject(transform);

        bool objetABouge = distanceDeplacee > positionTolerance;
        bool objetAChangéDeSalle = (roomActuelle != roomInitiale);

        if ((objetABouge || objetAChangéDeSalle) && !hasBeenMoved && !investigationInProgress)
        {
            hasBeenMoved = true;
            investigationInProgress = true;  // ← Bloque les investigations suivantes
            Debug.Log($"[ObjectMonitor] {gameObject.name} a changé ! Position initiale: {roomInitiale}, Position actuelle: {roomActuelle}");
            
            // Lance le coroutine d'investigation avec délai
            if (investigationCoroutine != null)
                StopCoroutine(investigationCoroutine);
            
            investigationCoroutine = StartCoroutine(InvestigateAfterDelay());
        }
    }

    IEnumerator InvestigateAfterDelay()
    {
        yield return new WaitForSeconds(detectionDelay);

        string roomActuelle = RoomManager.Instance.GetRoomForObject(transform);
        
        // Si l'objet a quitté sa salle initiale (soit hors de toutes les salles, soit dans une autre salle)
        bool objetAQuitteSaSalle = (roomActuelle != roomInitiale);
        
        if (objetAQuitteSaSalle)
        {
            Debug.Log($"[ObjectMonitor] Investigation lancée pour {gameObject.name} - absent de {roomInitiale}, actuellement dans: {roomActuelle ?? "HORS SALLES"}");
            
            // Notifie les NPCs qu'il faut investiguer la salle INITIALE (où devrait être l'objet)
            PNJ_Investigation investigationSystem = Object.FindAnyObjectByType<PNJ_Investigation>();
            if (investigationSystem != null)
            {
                investigationSystem.InvestigateRoom(roomInitiale, this);
            }
            else
            {
                Debug.LogWarning("[ObjectMonitor] Aucun PNJ_Investigation trouvé dans la scène!");
            }
        }
    }

    /// <summary>
    /// Réinitialise l'objet à sa position d'origine
    /// </summary>
    public void ResetToInitialPosition()
    {
        transform.position = positionInitiale;
        
        if (GetComponent<Rigidbody>() != null)
        {
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }

        Debug.Log($"[ObjectMonitor] {gameObject.name} réinitialisé à sa position d'origine");
    }

    /// <summary>
    /// Retourne la salle initiale de l'objet
    /// </summary>
    public string GetInitialRoom()
    {
        return roomInitiale;
    }

    public Vector3 GetInitialPosition()
    {
        return positionInitiale;
    }

    /// <summary>
    /// Appelée par PNJ_Investigation quand l'investigation est terminée
    /// Réactive le monitoring pour les changements futurs
    /// </summary>
    public void OnInvestigationComplete()
    {
        // Attendre que l'objet soit vraiment stabilisé à sa position initiale
        StartCoroutine(ReactivateMonitoringAfterDelay());
    }

    IEnumerator ReactivateMonitoringAfterDelay()
    {
        float stabilizationTimeout = 5f;
        float elapsedTime = 0f;
        
        // Attend que l'objet soit stable dans sa position initiale
        while (elapsedTime < stabilizationTimeout)
        {
            elapsedTime += Time.deltaTime;
            
            float distanceFromInitial = Vector3.Distance(transform.position, positionInitiale);
            string currentRoom = RoomManager.Instance.GetRoomForObject(transform);
            
            // Vérifie que l'objet est:
            // 1. Revenu dans sa salle initiale
            // 2. À sa position initiale (avec tolérance)
            // 3. La vélocité est stabilisée (proche de zéro)
            Rigidbody rb = GetComponent<Rigidbody>();
            float velocityMagnitude = rb != null ? rb.linearVelocity.magnitude : 0f;
            
            bool isInCorrectRoom = (currentRoom == roomInitiale);
            bool isInCorrectPosition = (distanceFromInitial <= positionTolerance);
            bool isStabilized = (velocityMagnitude < 0.1f);
            
            if (isInCorrectRoom && isInCorrectPosition && isStabilized)
            {
                Debug.Log($"[ObjectMonitor] {gameObject.name} - Stabilisé dans {roomInitiale}, monitoring réactivé");
                hasBeenMoved = false;
                investigationInProgress = false;
                yield break;
            }
            
            yield return null;
        }
        
        Debug.LogWarning($"[ObjectMonitor] {gameObject.name} - Timeout stabilisation, réactivation forcée");
        hasBeenMoved = false;
        investigationInProgress = false;
    }
}
