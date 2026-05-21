using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gère les salles du niveau et détecte dans quelle salle se trouve un objet
/// Singleton
/// </summary>
public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    [SerializeField] private float roomDetectionRadius = 0.5f;

    private Dictionary<string, Collider> rooms = new Dictionary<string, Collider>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        FindAllRooms();
    }

    /// <summary>
    /// Trouve toutes les salles (objets avec "Room_" ou "Salon_" dans le nom)
    /// </summary>
    void FindAllRooms()
    {
        Collider[] allColliders = FindObjectsOfType<Collider>();
        
        foreach (Collider col in allColliders)
        {
            string name = col.gameObject.name;
            if ((name.StartsWith("Room_") || name.StartsWith("Salon_")) && col.isTrigger)
            {
                rooms[name] = col;
                Debug.Log($"[RoomManager] Salle trouvée : {name}");
            }
        }

        Debug.Log($"[RoomManager] Total : {rooms.Count} salles détectées");
    }

    /// <summary>
    /// Détermine dans quelle salle se trouve un objet
    /// </summary>
    public string GetRoomForObject(Transform objectTransform)
    {
        Vector3 objectPos = objectTransform.position;

        foreach (var kvp in rooms)
        {
            Collider roomCollider = kvp.Value;
            
            // Vérifie si l'objet est dans le collider trigger
            if (roomCollider.bounds.Contains(objectPos))
            {
                return kvp.Key;
            }
        }

        return null; // Objet hors des salles
    }

    /// <summary>
    /// Retourne la position du centre d'une salle
    /// </summary>
    public Vector3? GetRoomCenter(string roomName)
    {
        if (rooms.ContainsKey(roomName))
        {
            return rooms[roomName].bounds.center;
        }
        return null;
    }

    /// <summary>
    /// Retourne le waypoint le plus proche d'une salle
    /// </summary>
    public Transform GetNearestWaypointToRoom(string roomName)
    {
        Vector3? roomCenter = GetRoomCenter(roomName);
        if (roomCenter == null) return null;

        Transform[] waypoints = FindObjectsOfType<Transform>();
        Transform nearestWaypoint = null;
        float minDistance = float.MaxValue;

        foreach (Transform wp in waypoints)
        {
            if (wp.name.StartsWith("WP_"))
            {
                float distance = Vector3.Distance(wp.position, roomCenter.Value);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestWaypoint = wp;
                }
            }
        }

        return nearestWaypoint;
    }
}
