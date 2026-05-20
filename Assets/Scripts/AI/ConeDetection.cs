using UnityEngine;

public class ConeDetection : MonoBehaviour
{
    [Header("Cone de vision")]
    public float distance = 5f;
    public float angle = 45f;
    public Transform joueur;

    private bool voyaitJoueur = false;
    private float angleOriginal;
    private float distanceOriginale;

    void Start()
    {
        angleOriginal = angle;
        distanceOriginale = distance;
    }

    public void SetModeAlerte(bool actif)
    {
        angle = actif ? 180f : angleOriginal;
        distance = actif ? distanceOriginale * 2f : distanceOriginale;
    }

    void Update()
    {
        bool voit = JoueurDetecte();

        if (JaugeDetection.Instance != null)
        {
            if (voit && !voyaitJoueur)
                JaugeDetection.Instance.SignalerDetection(true);
            else if (!voit && voyaitJoueur)
                JaugeDetection.Instance.SignalerDetection(false);
        }

        voyaitJoueur = voit;
    }

    bool JoueurDetecte()
    {
        if (joueur == null) return false;

        Vector3 monPosition = transform.position;
        Vector3 posJoueur = joueur.position;

        Vector3 direction = posJoueur - monPosition;
        float dist = direction.magnitude;
        if (dist > distance) return false;

        Transform reference = transform.parent != null ? transform.parent : transform;
        float angleDiff = Vector3.Angle(reference.forward, direction);
        if (angleDiff > angle / 2f) return false;

        // Ignore le layer PNJ pour ne pas se detecter soi-meme.
        int layerMask = ~LayerMask.GetMask("PNJ");

        RaycastHit hit;
        if (Physics.Raycast(monPosition, direction.normalized, out hit, distance, layerMask))
        {
            if (!hit.transform.CompareTag("Player"))
                return false;
        }

        return true;
    }

    void OnDrawGizmos()
    {
        Transform reference = transform.parent != null ? transform.parent : transform;
        Gizmos.color = voyaitJoueur ? Color.red : Color.yellow;
        Vector3 rayonGauche = Quaternion.Euler(0, -angle / 2f, 0) * reference.forward;
        Vector3 rayonDroit = Quaternion.Euler(0, angle / 2f, 0) * reference.forward;
        Gizmos.DrawRay(transform.position, rayonGauche * distance);
        Gizmos.DrawRay(transform.position, rayonDroit * distance);
        Gizmos.DrawWireSphere(transform.position, distance);
    }
}
